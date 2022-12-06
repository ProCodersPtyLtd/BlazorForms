using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class ExecutableRuleDetails
    {
        public Type RuleClassType { get; set; }
        public IFlowRule Instance { get; set; }
        public string RuleCode { get; set; }
    }
}
