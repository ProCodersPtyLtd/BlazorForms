using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform
{
    public class FieldDisplayDetails
    {
        //public string ModelBinding { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Caption { get; set; }
        public bool? Visible { get; set; }
        public bool? Disabled { get; set; }
        public bool? Required { get; set; }
        public bool? Highlighted { get; set; }
        public string Hint { get; set; }

        // new binding concept
        public FieldBinding Binding { get; set; }
    }
}
