using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlazorForms.Expressions
{
    public class SqlTokenizer
    {
        private readonly IObjectResolver _resolver;
        private readonly HashSet<string> _operators;

        public SqlTokenizer(TextReader reader, IObjectResolver resolver)
        {
            _resolver = resolver;
            _operators = _resolver.GetOperators().ToHashSet();

            _reader = reader;
            NextChar();
            NextToken();
        }

        TextReader _reader;
        char _currentChar;
        Token _currentToken;

        public string Operator { get; private set; }
        public string Field { get; private set; }
        public string Param { get; private set; }
        public decimal Number { get; private set; }
        public string StringLiteral { get; private set; }

        public Token Token
        {
            get { return _currentToken; }
        }

        public TokenInfo Info
        {
            get
            {
                return new TokenInfo
                {
                    Field = Field,
                    Number = Number,
                    Operator = Operator,
                    Param = Param,
                    Token = Token,
                    StringLiteral = StringLiteral
                };
            }
        }

        // Read the next character from the input strem
        // and store it in _currentChar, or load '\0' if EOF
        void NextChar()
        {
            CleanTokenProps();
            int ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        private void CleanTokenProps()
        {
            Field = null; Number = 0; Operator = null; Param = null; StringLiteral = null;
        }

        // Read the next token from the input stream
        public void NextToken()
        {
            // Skip whitespace
            while (char.IsWhiteSpace(_currentChar))
            {
                NextChar();
            }

            if (_operators.Contains(_currentChar.ToString().ToLower()))
            {
                _currentToken = Token.Operator;
                var current = _currentChar.ToString();
                NextChar();
                Operator = current;
                return;
            }

            // Special characters
            switch (_currentChar)
            {
                case '\0':
                    _currentToken = Token.EOF;
                    return;

                //case '+':
                //    NextChar();
                //    _currentToken = Token.Add;
                //    return;

                //case '-':
                //    NextChar();
                //    _currentToken = Token.Subtract;
                //    return;

                //case '*':
                //    NextChar();
                //    _currentToken = Token.Multiply;
                //    return;

                //case '/':
                //    NextChar();
                //    _currentToken = Token.Divide;
                //    return;

                case '(':
                    NextChar();
                    _currentToken = Token.OpenParens;
                    return;

                case ')':
                    NextChar();
                    _currentToken = Token.CloseParens;
                    return;

                case ',':
                    NextChar();
                    _currentToken = Token.Comma;
                    return;
            }

            // Number?
            if (char.IsDigit(_currentChar) || _currentChar == '.')
            {
                // Capture digits/decimal point
                var sb = new StringBuilder();
                bool haveDecimalPoint = false;
                while (char.IsDigit(_currentChar) || (!haveDecimalPoint && _currentChar == '.'))
                {
                    sb.Append(_currentChar);
                    haveDecimalPoint = _currentChar == '.';
                    NextChar();
                }

                // Parse it
                Number = decimal.Parse(sb.ToString(), CultureInfo.InvariantCulture);
                _currentToken = Token.Number;
                return;
            }

            // Field - starts with letter or underscore
            // also may be operator
            if (char.IsLetter(_currentChar) || _currentChar == '_')
            {
                var sb = new StringBuilder();

                // Accept letter, digit or underscore
                while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_' || _currentChar == '.')
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                var sbs = sb.ToString();

                if (_operators.Contains(sbs.ToLower()))
                {
                    _currentToken = Token.Operator;
                    Operator = sbs;
                    return;
                }

                // Setup token
                Field = sbs;
                _currentToken = Token.Field;
                return;
            }

            // Param - starts with letter or underscore
            if (_currentChar == '@')
            {
                var sb = new StringBuilder();
                sb.Append(_currentChar);
                NextChar();

                // starts from letters and _
                if (char.IsLetter(_currentChar) || _currentChar == '_')
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                // Accept letter, digit or underscore
                while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_')
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                // Setup token
                Param = sb.ToString();
                _currentToken = Token.Param;
                return;
            }

            // string literal
            if (_currentChar == '\'')
            {
                var sb = new StringBuilder();
                NextChar();

                while (_currentChar != '\'')
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                NextChar();
                StringLiteral = sb.ToString();
                _currentToken = Token.StringLiteral;
                return;
            }


            throw new Exception($"Illigal literal '{_currentChar}'");
        }
    }

    public class TokenInfo
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public string Param { get; set; }
        public string StringLiteral { get; set; }
        public decimal Number { get; set; }
        public Token Token { get; set; }
        public List<TokenInfo> Expression { get; set; }

        public void Print(StringBuilder sb)
        {
            switch (Token)
            {
                case Token.Param: sb.Append($"{Param}"); break;
                case Token.Number: sb.Append($"{Number}"); break;
                case Token.Operator: sb.Append($"{Operator}"); break;
                case Token.StringLiteral: sb.Append($"'{StringLiteral}'"); break;
                case Token.Field: sb.Append($"{Field}"); break;
            }
        }
    }

    public enum Token
    {
        EOF,
        Operator,
        Field,
        Param,
        OpenParens,
        CloseParens,

        Add,
        Subtract,
        Multiply,
        Divide,
        Comma,
        //Identifier,
        Number,
        StringLiteral,

        // used for expressions
        ParensExpression,
        MathExpression
    }

    public interface IObjectResolver
    {
        string[] GetQueryFields();
        string[] GetQueryParams();
        string[] GetOperators();
        Dictionary<int, string[]> GetPriorityOperators();
    }
}
