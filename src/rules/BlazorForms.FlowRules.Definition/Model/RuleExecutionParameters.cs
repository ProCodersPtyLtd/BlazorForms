using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlazorForms.FlowRules
{
    public class RuleExecutionParameters
    {
        public object Model { get; set; }
        public object AccessModel { get; set; }
        public FormRuleTriggers? TriggeredTriggerType { get; set; }
        public string TriggeredRuleCode { get; set; }
        //public string TriggeredFieldJsonPath { get; set; }
        public FieldBinding TriggeredFieldBinding { get; set; }
        public string ProcessTaskTypeFullName { get; set; }
        public Dictionary<string, DisplayDetails> FieldsDisplayProperties { get; set; }
        public int RowIndex { get; set; }
        public string AttachedFieldBinding { get; set; }
        public FlowParamsGeneric FlowParams { get; set; }
        public Collection<FieldDetails> Fields { get; set; }
    }
}
