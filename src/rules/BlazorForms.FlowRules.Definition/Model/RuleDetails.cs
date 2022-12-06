using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class RuleDetails
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string RuleCode { get; set; }
        public RuleTypes RuleType { get; set; }
        public FormRuleTriggers RuleTriggerType { get; set; }
        public bool IsOuterProperty { get; set; }
    }
}
