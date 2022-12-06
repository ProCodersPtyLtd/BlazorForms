using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.Platform
{
    public class FieldDetails
    {
        public string Name { get; set; }
        public string ControlType { get; set; }
        public Collection<FieldRuleDetails> AttachedFlowRules { get; set; }

        // bindings
        public string ModelBinding { get; set; }
        public string ModelBindingType { get; set; }
        public string ModelItems { get; set; }
        public string ModelItemId { get; set; }
        public string ModelItemName { get; set; }
        public string ModelTable { get; set; }

        // new binding concept
        public FieldBinding Binding { get; set; }
    }
}
