using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class DisplayDetails
    {
        //public string ModelBinding { get; set; }
        public FieldBinding Binding { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public bool Visible { get; set; }
        public bool Disabled { get; set; }
        public bool Required { get; set; }
        public bool Highlighted { get; set; }
        public string Hint { get; set; }

        public DisplayDetails Set(Action<DisplayDetails> action)
        {
            action(this);
            return this;    
        }
    }
}
