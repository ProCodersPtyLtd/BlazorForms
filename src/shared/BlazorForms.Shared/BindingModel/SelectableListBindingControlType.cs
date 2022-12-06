using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class SelectableListBindingControlType : IBindingControlType
    {
        public string Binding { get; set; }

        public string TargetBinding { get; set; }
    }
}
