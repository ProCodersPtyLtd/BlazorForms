using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
    
[assembly: InternalsVisibleTo("BlazorForms.FlowRules")]
namespace BlazorForms.FlowRules
{
    public class RuleEngineExecutionResultNoModel
    {
        public AccessRuleModel AccessModel { get; set; }

        public List<RuleExecutionResult> Validations { get; set; }

        public Dictionary<string, DisplayDetails> FieldsDisplayProperties { get; set; }
        public bool SkipThisChange { get; set; }
    }

    public class RuleEngineExecutionResult : RuleEngineExecutionResultNoModel
    {
        public object Model { get; set; }

        public RuleEngineExecutionResult()
        { }

        public RuleEngineExecutionResult(RuleEngineExecutionResultNoModel source, object model)
        {
            AccessModel = source.AccessModel;
            Validations = source.Validations;
            FieldsDisplayProperties = source.FieldsDisplayProperties;
            Model = model;
        }
    }
}
