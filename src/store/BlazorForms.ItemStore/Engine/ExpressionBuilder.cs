using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class ExpressionBuilder
    {
        private readonly Dictionary<string, Func<QueryExpression, string>> _operations;
        private readonly IStoreObjectResolver _resolver;
        public ExpressionBuilder(IStoreObjectResolver resolver)
        {
            _resolver = resolver;

            _operations = new Dictionary<string, Func<QueryExpression, string>>()
            {
                ["field"] = FieldOperation,
                ["const"] = ConstOperation,
                ["param"] = ParamOperation,
                ["="] = EqualOperation,
                // logical
                ["true"] = TrueOperation,
                ["and"] = AndOperation,
                ["or"] = OrOperation,
            };
        }

        public string Build(QueryExpression exp)
        {
            return _operations[exp.Operator](exp);
        }

        private string TrueOperation(QueryExpression exp)
        {
            var left = Build(exp.Left);
            return $"{left}";
        }

        private string AndOperation(QueryExpression exp)
        {
            var left = Build(exp.Left);
            var right = Build(exp.Right);
            return $"({left}) AND ({right})";
        }
        private string OrOperation(QueryExpression exp)
        {
            var left = Build(exp.Left);
            var right = Build(exp.Right);
            return $"({left}) OR ({right})";
        }

        private string ParamOperation(QueryExpression exp)
        {
            var value = exp.ScalarParam.ToString();
            return $"{value}";
        }
        private string ConstOperation(QueryExpression exp)
        {
            var value = exp.ScalarParam.ToString();
            return $"'{value}'";
        }

        private string EqualOperation(QueryExpression exp)
        {
            var left = Build(exp.Left);
            var right = Build(exp.Right);
            return $"{left} = {right}";
        }
        private string FieldOperation(QueryExpression exp)
        {
            var field = exp.FieldParam;
            //return $"JSON_VALUE({field.TableAlias}.{SqlScriptHelper.DATA_COLUMN}, '$.{field.Name}')";
            var sql = _resolver.ResolveField(field.Field);
            return sql;
        }
    }

}
