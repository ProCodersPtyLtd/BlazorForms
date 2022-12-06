using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorForms.FlowRules
{
    public enum RuleTypes
    {
        [Description("display")]
        DisplayRule = 1,
        [Description("validation")]
        ValidationRule,
        [Description("access")]
        AccessRule
    }
}
