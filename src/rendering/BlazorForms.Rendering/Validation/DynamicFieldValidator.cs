using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Validation
{
    public interface IDynamicFieldValidator
    {
        RuleExecutionResult Validate(FieldControlDetails field, object value);
        IEnumerable<RuleExecutionResult> Validate(FieldControlDetails field, object value, object model);
        IEnumerable<RuleExecutionResult> PrepareValidations(IEnumerable<RuleExecutionResult> validations, RuleExecutionResult localValidation);
    }
    public class DynamicFieldValidator : IDynamicFieldValidator
    {
        private readonly IJsonPathNavigator _jsonPathNavigator;

        public DynamicFieldValidator(IJsonPathNavigator jsonPathNavigator)
        {
            _jsonPathNavigator = jsonPathNavigator;
        }

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
            return ValidateRequired(field, value);
        }

        public IEnumerable<RuleExecutionResult> Validate(FieldControlDetails field, object value, object model)
        {
            var result = new List<RuleExecutionResult>();
            var required = ValidateRequired(field, value);

            if (required != null)
            { 
                result.Add(required); 
            }

            var itemExists = ValidateItemExists(field, value, model);

            if (itemExists != null)
            {
                result.Add(itemExists);
            }

            return result;
        }

        private RuleExecutionResult ValidateItemExists(FieldControlDetails field, object value, object model)
        {
            if (field?.ControlType == "Autocomplete" && field.DisplayProperties.Visible && value != null)
            {
                var options = _jsonPathNavigator.GetItems(model, field.Binding.ItemsBinding);

                if (!options.Any(x => value.ToString() == _jsonPathNavigator.GetValue(x, field.Binding.NameBinding).ToString()))
                {
                    return new RuleExecutionResult
                    {
                        AffectedField = field.Binding.Key,
                        RuleCode = field.DisplayProperties.Caption,
                        ValidationMessage = "This is an incorrect value",
                        ValidationResult = RuleValidationResult.Error
                    };
                }
            }
                
            return null;
        }

        private RuleExecutionResult ValidateRequired(FieldControlDetails field, object value) 
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

                // ToDo: remove this hardcode during validation refactoring
                if ((field.ControlType == "DropDown" || field.ControlType == "DropDownSearch") && (value == null || value.ToString() == "0"))
                {
                    failed = true;
                }

                if (failed)
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
        }
    }
}
