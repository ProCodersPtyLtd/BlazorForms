using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class RuleExecutionResult
    {
        public RuleValidationResult ValidationResult { get; set; }
        public string ValidationMessage { get; set; }
        public bool SkipThisChange { get; set; }
        //public DynamicParams DynamicProperties { get; set; }
        public AutoDictionary<string, DisplayDetails> Fields { get; } = new AutoDictionary<string, DisplayDetails>();
        public string RuleCode { get; set; }
        public string AffectedField { get; set; }
        //public FieldBinding AffectedField { get; set; }
        public List<string> ChangedFields { get; } = new List<string>();

        internal void Clear()
        {
            ChangedFields.Clear();
            ValidationMessage = null;
            ValidationResult = RuleValidationResult.Ok;
        }

        internal void Initialize(Dictionary<string, DisplayDetails> fieldsDisplayProperties)
        {
            Clear();

            if (fieldsDisplayProperties != null)
            {
                Fields.Dictionary = fieldsDisplayProperties;
            }
        }

    }
}
