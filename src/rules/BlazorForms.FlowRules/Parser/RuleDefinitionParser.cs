using BlazorForms.FlowRules;
using BlazorForms.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.FlowRules
{
    public class RuleDefinitionParser : IRuleDefinitionParser
    {
        public const string GlobalMapping = ModelBinding.FormLevelBinding;
        public const string ColumnIndexMarker = "[__index]";

        private readonly IServiceProvider _serviceProvider;

        public RuleDefinitionParser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<ExecutableRuleDetails> FindAllRules(IEnumerable<Assembly> assemblies)
        {
            var binder = _serviceProvider.GetService<IKnownTypesBinder>();

            var types = assemblies.SelectMany(a => a.GetTypes()).Union(binder.KnownTypes)
                .Where(x => typeof(IFlowRule).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract && !x.ContainsGenericParameters);

            var rules = types.Select(t => new ExecutableRuleDetails
            {
                RuleClassType = t,
                Instance = CreateRuleInstance(t)
            });

            var result = rules.ToList();
            result.ForEach(r => r.RuleCode = r.Instance.RuleCode);
            return result;
        }

        public IFlowRule CreateRuleInstance(Type t)
        {
            // ToDo: add constructor parameters
            var ruleParameters = TypeHelper.GetConstructorParameters(_serviceProvider, t);

            if (t.ContainsGenericParameters)
            {
                //t = t.MakeGenericType(t = t.GenericTypeParameters);
            }

            return Activator.CreateInstance(t, ruleParameters) as IFlowRule;
        }

        public TaskRulesDefinition Parse(Type taskType)
        {
            var details = new TaskRulesDefinition();

            if (!typeof(ITaskRulesDefinition).IsAssignableFrom(taskType))
            {
                details.Fields = new Collection<FieldDetails>();
                return details;
            }

            var parameters = TypeHelper.GetConstructorParameters(_serviceProvider, taskType);
            var task = Activator.CreateInstance(taskType, parameters) as ITaskRulesDefinition;

            details.ProcessTaskTypeFullName = task.ProcessTaskTypeFullName;
            details.Name = task?.Name ?? taskType.Name;
            var boundFields = taskType.GetProperties().Where(t => t.GetCustomAttributes().Any(attr => attr.GetType() == typeof(FlowRuleAttribute)));
            details.Fields = new Collection<FieldDetails>();

            foreach (var field in boundFields)
            {
                var fieldValue = field.GetValue(task);
                var bindingControl = fieldValue as IBindingControlType;

                // read as FieldBinding
                var fieldBinding = fieldValue as FieldBinding;
                var defAttrs = field.GetCustomAttributes().Where(attr => attr.GetType() == typeof(FlowRuleAttribute));

                var newField = new FieldDetails
                {
                    Rules = new Collection<RuleDetails>(),

                    // new binding concept
                    Binding = fieldBinding
                };

                foreach(FlowRuleAttribute attr in defAttrs)
                {
                    var rule = CreateRuleInstance(attr.RuleType);

                    var newRule = new RuleDetails
                    {
                        Name = attr.Name,
                        FullName = attr.FullName,
                        RuleCode = rule.RuleCode,
                        RuleType = rule.RuleType,
                        RuleTriggerType = attr.Trigger
                    };


                    newField.Rules.Add(newRule);
                }

                details.Fields.Add(newField);
            }

            // global rules
            var taskAttrs = taskType.GetCustomAttributes().Where(attr => attr.GetType() == typeof(FlowRuleAttribute));

            var globalField = new FieldDetails
            {
                Binding = new FieldBinding { Binding = GlobalMapping, },
                Rules = new Collection<RuleDetails>()
            };

            foreach (FlowRuleAttribute attr in taskAttrs)
            {
                var rule = CreateRuleInstance(attr.RuleType);

                var newRule = new RuleDetails
                {
                    Name = attr.Name,
                    FullName = attr.FullName,
                    RuleCode = rule.RuleCode,
                    RuleType = rule.RuleType,
                    RuleTriggerType = attr.Trigger
                };


                globalField.Rules.Add(newRule);
            }

            details.Fields.Add(globalField);

            return details;
        }
    }
}
