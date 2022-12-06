using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlazorForms.Platform
{
    public class RuleExecutionRequest
    {
        public string ProcessTaskTypeFullName { get; set; }
        public string RuleCode { get; set; }
        public FormRuleTriggers? Trigger { get; set; }
        public FieldBinding FieldBinding { get; set; }
        public int RowIndex { get; set; }
        public string AttachedFieldBinding { get; set; }
        public FlowParamsGeneric FlowParams { get; set; }
        public Collection<FieldDetails> Fields { get; set; }
        public FieldDisplayDetails[] DisplayProperties { get; set; }
    }
}
