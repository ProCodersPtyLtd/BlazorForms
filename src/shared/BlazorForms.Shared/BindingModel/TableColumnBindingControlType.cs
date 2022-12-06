using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class TableBindingControlType : IBindingControlType
    {
        public string Binding { get; set; }

        public string TableBinding { get; set; }
    }

    public class TableColumnBindingControlType : IBindingControlType
    {
        public string Binding { get; set; }

        public string TableBinding { get; set; }
    }
}
