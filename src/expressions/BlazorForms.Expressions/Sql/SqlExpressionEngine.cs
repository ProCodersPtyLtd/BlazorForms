using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlazorForms.Expressions
{
    public class SqlExpressionEngine
    {
        private readonly IObjectResolver _resolver;
        public SqlExpressionEngine(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public Expr BuildExpressionTree(string text)
        {
            var tokenList = new List<TokenInfo>();
            int tokenListNumber = 0;

            // 1. read all tokens and build initial tree by Parens
            using (TextReader sr = new StringReader(text))
            {
                var t = new SqlTokenizer(sr, _resolver);
                var current = t.Token;
                var openParenStack = new List<int>();

                while (current != Token.EOF)
                {
                    tokenList.Add(t.Info);

                    // read expression
                    if (current == Token.OpenParens)
                    {
                        openParenStack.Add(tokenListNumber);
                    }

                    if (current == Token.CloseParens)
                    {
                        if (openParenStack.Count < 1)
                        {
                            throw new Exception($"Cannot find open parens for close parens ')'");
                        }

                        var openParensIndex = openParenStack.Last();
                        openParenStack.RemoveAt(openParenStack.Count - 1);
                        //var sb = new StringBuilder();
                        var expr = new List<TokenInfo>();

                        for (int i = openParensIndex + 1; i <= tokenListNumber; i++)
                        {
                            var ti = tokenList[i];
                            //sb.Append($"{ti.Field}{ti.Param}{ti.Operator}{ti.Number}{ti.StringLiteral}");

                            if (ti != null && ti.Token != Token.CloseParens)
                            {
                                expr.Add(ti);
                                tokenList[i] = null;
                            }

                            //if(ti?.Token == Token.CloseParens)
                            //{
                            //    tokenList[i] = null;
                            //}
                        }

                        tokenList[openParensIndex] = new TokenInfo { Token = Token.ParensExpression, Expression = expr };
                        tokenList[tokenListNumber] = null;
                    }

                    // next
                    t.NextToken();
                    current = t.Token;
                    tokenListNumber++;
                }

                tokenList = tokenList.Where(t => t != null && t.Token != Token.CloseParens).ToList();
            }

            // 2. Apply logical operators in order : = AND OR
            var prioDict = _resolver.GetPriorityOperators();
            var min = prioDict.Keys.Min();
            var max = prioDict.Keys.Max();

            for (int i = min; i <= max; i++)
            {
                ApplyOperators(tokenList, prioDict[i]);
            }

            var sb = new StringBuilder();
            PrintExpressions(tokenList, sb);
            Console.WriteLine(sb.ToString());

            var mainExpr = new Expr { Type = ExprType.Expr };
            BuildTree(tokenList, mainExpr);
            return mainExpr;
        }

        private void BuildTree(List<TokenInfo> tokenList, Expr expr)
        {
            if (tokenList.Count == 1)
            {
                switch (tokenList[0].Token)
                {
                    case Token.ParensExpression:
                    case Token.MathExpression:
                        expr.Type = ExprType.Expr;
                        BuildTree(tokenList[0].Expression, expr);
                        return;
                    case Token.Param:
                        expr.Type = ExprType.Param;
                        expr.Value = tokenList[0].Param;
                        return;
                    case Token.Field:
                        expr.Type = ExprType.Field;
                        expr.Value = tokenList[0].Field;
                        return;
                    case Token.Number:
                        expr.Type = ExprType.Number;
                        expr.Value = $"{tokenList[0].Number}";
                        return;
                    case Token.StringLiteral:
                        expr.Type = ExprType.StringLiteral;
                        expr.Value = $"'{tokenList[0].StringLiteral}'";
                        return;
                }
            }
            else if (tokenList.Count == 3)
            {
                expr.Left = new Expr();
                BuildTree(new TokenInfo[] { tokenList[0] }.ToList(), expr.Left);
                expr.Operator = tokenList[1].Operator;
                expr.Right = new Expr();
                BuildTree(new TokenInfo[] { tokenList[2] }.ToList(), expr.Right);
            }
        }

        public class Expr
        {
            public ExprType Type { get; set; }
            public Expr Left { get; set; }
            public string Value { get; set; }
            public string Operator { get; set; }
            public Expr Right { get; set; }

            public bool IsExprPart { get { return Type != ExprType.None && Type != ExprType.Expr; } }

            public ExprValue GetExprValue()
            {
                switch (this.Type)
                {
                    case ExprType.Expr:
                        var left = this.Left.GetExprValue();
                        var right = this.Right.GetExprValue();
                        return CalcExpression(left, this.Operator, right);
                    case ExprType.Number:
                        return new ExprValue { Type = ExprValueType.Number, Value = Convert.ToDecimal(this.Value) };
                    case ExprType.StringLiteral:
                        return new ExprValue { Type = ExprValueType.String, Value = Convert.ToString(this.Value) };
                }

                throw new NotImplementedException("Other expression types not supported currently");
            }

            private ExprValue CalcExpression(ExprValue left, string oper, ExprValue right)
            {
                switch (oper)
                {
                    case "+": return new ExprValue { Type = ExprValueType.Number, Value = (decimal)left.Value + (decimal)right.Value };
                    case "-": return new ExprValue { Type = ExprValueType.Number, Value = (decimal)left.Value - (decimal)right.Value };
                    case "=":
                        if (left.Type == ExprValueType.Number && right.Type == ExprValueType.Number)
                        {
                            return new ExprValue { Type = ExprValueType.Bool, Value = (decimal)left.Value == (decimal)right.Value };
                        }
                        else if (left.Type == ExprValueType.Bool && right.Type == ExprValueType.Bool)
                        {
                            return new ExprValue { Type = ExprValueType.Bool, Value = (bool)left.Value == (bool)right.Value };
                        }
                        else if (left.Type == ExprValueType.String && right.Type == ExprValueType.String)
                        {
                            return new ExprValue { Type = ExprValueType.Bool, Value = (string)left.Value == (string)right.Value };
                        }
                        throw new NotImplementedException("Other type comparison not supported currently");

                    case "AND": return new ExprValue { Type = ExprValueType.Bool, Value = (bool)left.Value && (bool)right.Value };
                    case "OR": return new ExprValue { Type = ExprValueType.Bool, Value = (bool)left.Value || (bool)right.Value };
                    default:
                        throw new NotImplementedException("Other operators not supported currently");
                }
            }
        }

        public class ExprValue
        {
            public ExprValueType Type { get; set; }
            public object Value { get; set; }
        }

        public enum ExprValueType
        {
            Bool,
            Number,
            String,
            Date
        }

        public enum ExprType
        {
            None,
            Expr,
            Field,
            Param,
            Number,
            StringLiteral,
        }

        private void PrintExpressions(List<TokenInfo> tokenList, StringBuilder sb)
        {
            int i = 0;

            while (i < tokenList.Count)
            {
                var ti = tokenList[i];

                if (ti.Token == Token.ParensExpression || ti.Token == Token.MathExpression)
                {
                    sb.AppendLine();
                    PrintExpressions(ti.Expression, sb);
                }

                if (ti.Token == Token.Operator && (ti.Operator == "AND" || ti.Operator == "OR"))
                {
                    sb.AppendLine();
                    sb.Append("left");
                    ti.Print(sb);
                    sb.AppendLine("right");
                }
                else
                {
                    ti.Print(sb);
                }

                i++;
            }
        }

        private void ApplyOperators(List<TokenInfo> tokenList, string[] operators)
        {
            int i = 0;

            while (i < tokenList.Count)
            {
                var ti = tokenList[i];

                if (ti.Token == Token.ParensExpression || ti.Token == Token.MathExpression)
                {
                    ApplyOperators(ti.Expression, operators);
                }

                if (ti.Token == Token.Operator && operators.Contains(ti.Operator))
                {
                    // ToDo: only x=y operators supported, not supported unary operators like +x, -+-x, etc.
                    var expr = new List<TokenInfo>();

                    if (i - 1 < 0)
                    {
                        throw new Exception($"Cannot find left operand for operator '{ti.Operator}'");
                    }

                    if (i + 1 >= tokenList.Count)
                    {
                        throw new Exception($"Cannot find right operand for operator '{ti.Operator}'");
                    }

                    expr.Add(tokenList[i - 1]);
                    expr.Add(tokenList[i]);
                    expr.Add(tokenList[i + 1]);

                    i--;
                    tokenList.RemoveAt(i);
                    tokenList.RemoveAt(i);
                    tokenList[i] = new TokenInfo { Token = Token.MathExpression, Expression = expr };
                }

                i++;
            }

            //tokenList = tokenList.Where(t => t != null).ToList();
        }
    }

}
