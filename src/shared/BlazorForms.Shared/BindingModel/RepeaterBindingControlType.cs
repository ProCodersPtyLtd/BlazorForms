using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class RepeaterBindingControlType : IBindingControlType
    {
        public string Binding { get; set; }

        public string TableBinding { get; set; }
    }
}
