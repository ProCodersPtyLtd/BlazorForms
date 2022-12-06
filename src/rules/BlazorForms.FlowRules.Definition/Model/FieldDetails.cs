using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class FieldDetails
    {
        public Collection<RuleDetails> Rules { get; set; }

        // new binding concept
        public FieldBinding Binding { get; set; }
    }
}
