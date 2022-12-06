using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Expressions
{
    public class SqlJsonObjectResolver : IObjectResolver
    {
        public string[] GetOperators()
        {
            return new string[] { "*", "/", "+", "-", "=", "<>", ">", "<", "and", "or" };
        }

        public Dictionary<int, string[]> GetPriorityOperators()
        {
            return new Dictionary<int, string[]>
            {
                [1] = new string[] { "*", "/" },
                [2] = new string[] { "+", "-" },
                [3] = new string[] { "=", "<>", ">", "<" },
                [4] = new string[] { "and" },
                [5] = new string[] { "or" },
            };
        }

        public string[] GetQueryFields()
        {
            throw new NotImplementedException();
        }

        public string[] GetQueryParams()
        {
            throw new NotImplementedException();
        }
    }
}
