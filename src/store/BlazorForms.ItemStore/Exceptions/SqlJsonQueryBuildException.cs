using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class SqlJsonQueryBuildException: Exception
    {
        public SqlJsonQueryBuildException(string msg) : base(msg)
        { }
    }
}
