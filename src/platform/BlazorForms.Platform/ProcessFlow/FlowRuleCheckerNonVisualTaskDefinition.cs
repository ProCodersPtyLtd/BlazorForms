using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;

namespace BlazorForms.Platform.ProcessFlow
{
    public class FlowRuleCheckerNonVisualTaskDefinition<TSource> : NonVisualFlowTaskBase<TSource> where TSource : class
    {
        private readonly IRuleExecutionEngine _ruleExecutionEngine;

        public FlowRuleCheckerNonVisualTaskDefinition(IRuleExecutionEngine ruleExecutionEngine)
        {
            _ruleExecutionEngine = ruleExecutionEngine;
        }

        public override async Task Execute(IFlowContext context, Type taskType, ILogStreamer logStreamer)
        {
            // Execute Load rules
            var parameters = new RuleExecutionParameters { Model = Model, TriggeredTriggerType = FormRuleTriggers.Loaded, ProcessTaskTypeFullName = GetType().FullName };
            var loadResult = await _ruleExecutionEngine.Execute(parameters);

            // Execute Submit rules
            parameters = new RuleExecutionParameters { Model = Model, TriggeredTriggerType = FormRuleTriggers.Submit, ProcessTaskTypeFullName = GetType().FullName };
            var submitResult = await _ruleExecutionEngine.Execute(parameters);

            var loadValidations = loadResult.Validations.Select(v => new TaskExecutionValidationResult
            {
                ValidationMessage = v.ValidationMessage,
                ValidationResult = v.ValidationResult.GetDescription(),
                RuleCode = v.RuleCode,
                AffectedField = v.AffectedField
            });

            var submitValidations = submitResult.Validations.Select(v => new TaskExecutionValidationResult
            {
                ValidationMessage = v.ValidationMessage,
                ValidationResult = v.ValidationResult.GetDescription(),
                RuleCode = v.RuleCode,
                AffectedField = v.AffectedField
            });

            var result = loadValidations.Union(submitValidations);
            context.ExecutionResult.TaskExecutionValidationIssues.AddRange(result);
        }
    }
}
