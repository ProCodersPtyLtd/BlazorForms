using BlazorForms.FlowRules;
using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Validation
{
    public interface IDynamicFieldValidator
    {
        RuleExecutionResult Validate(FieldControlDetails field, object value);
        IEnumerable<RuleExecutionResult> PrepareValidations(IEnumerable<RuleExecutionResult> validations, RuleExecutionResult localValidation);
    }
    public class DynamicFieldValidator : IDynamicFieldValidator
    {
        public IEnumerable<RuleExecutionResult> PrepareValidations(IEnumerable<RuleExecutionResult> validations, RuleExecutionResult localValidation)
        {
            var result = new List<RuleExecutionResult>();

            if (localValidation != null)
            {
                result.Add(localValidation);
            }

            result.AddRange(validations);
            result = result
                .GroupBy(p => p.ValidationMessage)
                .Select(g => g.First())
                .ToList();
            return result;
        }

        public RuleExecutionResult Validate(FieldControlDetails field, object value)
        {
            if (field?.ControlType != null && field.DisplayProperties.Visible && field.DisplayProperties.Required)
            {
                bool failed = false;
                switch (field.ControlType)
                {
                    case "TextEdit":
                    case "DropDown":
                    case "Autocomplete":
                        failed = string.IsNullOrWhiteSpace(Convert.ToString(value));
                        break;
                    case "DateEdit":
                        failed = value == null;
                        break;
                }

                if(failed)
                {
                    return new RuleExecutionResult
                    {
                        AffectedField = field.Binding.Key,
                        RuleCode = field.DisplayProperties.Caption,
                        ValidationMessage = "This field is required",
                        ValidationResult = RuleValidationResult.Error
                    };
                }
            }

            return null;
            //return new RuleExecutionResult { ValidationResult = RuleValidationResult.Ok };
        }
    }
}
