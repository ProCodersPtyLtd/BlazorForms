using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class ListBindingControlType : IBindingControlType
    {
        public string Binding { get; set; }

        public string ItemsBinding { get; set; }

        public string IdBinding { get; set; }

        public string NameBinding { get; set; }
    }
}
