using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormFlowRuleDetails
    {
        public string FormRuleCode { get; set; }
        public string FormRuleType { get; set; }
        public string FormRuleTriggerType { get; set; }
        public bool IsOuterProperty { get; set; }
    }
}
