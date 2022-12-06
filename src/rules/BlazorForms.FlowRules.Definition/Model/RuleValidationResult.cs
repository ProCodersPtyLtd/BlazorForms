using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorForms.FlowRules
{
    public enum RuleValidationResult
    {
        [Description("ok")]
        Ok = 0,
        [Description("warning")]
        Warning = 1,
        [Description("error")]
        Error = 2,
    }
}
