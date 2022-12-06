using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class TaskExecutionValidationResult
    {
        public string RuleCode { get; set; }
        public string ValidationMessage { get; set; }
        public string ValidationResult { get; set; }
        //public FieldBinding AffectedField { get; set; }
        public string AffectedField { get; set; }
    }
}
