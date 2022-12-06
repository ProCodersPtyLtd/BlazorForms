using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public class QueryParameter
    {
        public string Name { get; set; }
        // int, string, date, etc.
        public string Type { get; set; }
        public object Value { get; set; }
    }
}
