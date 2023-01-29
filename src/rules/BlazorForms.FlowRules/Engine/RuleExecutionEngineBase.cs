using BlazorForms.Proxyma;
using BlazorForms.Shared;
using BlazorForms.Shared.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules.Engine
{
    public abstract class RuleExecutionEngineBase : IRuleExecutionEngine
    {
        protected readonly IJsonPathNavigator _navigator;
        protected readonly IEnumerable<Assembly> _asms;
        protected readonly Dictionary<string, Type> _allTypes;
        protected readonly IEnumerable<ExecutableRuleDetails> _allRules;
        protected readonly Dictionary<string, ExecutableRuleDetails> _rulesDictionary;
        protected readonly IRuleDefinitionParser _parser;
        protected readonly IAssemblyRegistrator _assemblyRegistrator;
        protected readonly ILogStreamer _logStreamer;
        protected readonly IKnownTypesBinder _binder;

        public RuleExecutionEngineBase(ILogStreamer logStreamer, IRuleDefinitionParser parser, IAssemblyRegistrator assemblyRegistrator,
            IJsonPathNavigator navigator, IKnownTypesBinder binder)
        {
            _logStreamer = logStreamer;
            _parser = parser;
            _assemblyRegistrator = assemblyRegistrator;
            _navigator = navigator;
            _binder = binder;
            _asms = _assemblyRegistrator.GetConsideredAssemblies().Distinct();

            _allTypes = _asms
                            .SelectMany(a => a.GetTypes())
                            .Union(_binder.KnownTypes)
                            .GroupBy(t => t.FullName)
                            .ToDictionary(t => t.First().FullName, t => t.First());

            try
            {
                _allRules = _parser.FindAllRules(_asms);

                var dups = _allRules.GroupBy(r => r.RuleCode).Where(g => g.Count() > 1);

                if (dups.Any())
                {
                    var codes = String.Join(",", dups.Select(d => d.Key));
                    throw new BlazorFormsValidationException($"Rule code must be unique, the following codes used more than once: {codes}");
                }

                _rulesDictionary = _allRules.ToDictionary(d => d.RuleCode, d => d);
            }
            catch (Exception exc)
            {
                _logStreamer.TrackException(exc);
                throw;
            }
        }

        public virtual async Task<RuleEngineExecutionResult> Execute(RuleExecutionParameters parameters)
        {
            var changedFields = new ConcurrentStack<string>();
            var result = await ExecuteLogic(parameters, parameters.Model, changedFields);
            return result;
        }

        protected async Task<RuleEngineExecutionResult> ExecuteLogic(RuleExecutionParameters parameters, 
            object modelProxy, ConcurrentStack<string> changedFields)
        {
            // validate parameters
            if (string.IsNullOrEmpty(parameters.TriggeredRuleCode) && parameters.TriggeredFieldBinding == null && parameters.TriggeredTriggerType == null)
            {
                throw new ArgumentException("TriggeredTriggerType or TriggeredRuleCode or TriggeredFieldJsonPath should be provided");
            }

            var currentFieldsDisplayProperties = parameters.FieldsDisplayProperties;
            //var changedFields = new ConcurrentStack<string>();
            var processedFileds = new Dictionary<string, int>();

            var result = new RuleEngineExecutionResult { Validations = new List<RuleExecutionResult>() };
            var taskType = _allTypes[parameters.ProcessTaskTypeFullName];
            var fields = parameters.Fields ?? _parser.Parse(taskType).Fields;

            // consider only rule trigger type that supplied or only type Changed
            Dictionary<string, IEnumerable<RuleDetails>> bindingDictionary;
            Dictionary<string, int> bindingRowIndexResolution;

            if (parameters.TriggeredTriggerType == null)
            {
                bindingDictionary = fields.ToDictionary(f => f.Binding.Key, f => f.Rules.Where(r => r.RuleTriggerType == FormRuleTriggers.Changed
                    || r.RuleTriggerType == FormRuleTriggers.ItemAdded || r.RuleTriggerType == FormRuleTriggers.ItemChanged
                    || r.RuleTriggerType == FormRuleTriggers.ItemDeleting));

                bindingRowIndexResolution = SpreadBindingRulesForLists(bindingDictionary, parameters.Model);
            }
            else
            {

                // for Submit include Changed rules too
                if (parameters.TriggeredTriggerType == FormRuleTriggers.Submit)
                {
                    bindingDictionary = fields.Where(f => f.Binding.BindingType != FieldBindingType.ActionButton).
                        ToDictionary(f => f.Binding.Key, f => f.Rules
                        .Where(r => r.RuleTriggerType == FormRuleTriggers.Submit || r.RuleTriggerType == FormRuleTriggers.Changed));
                }
                else
                {
                    bindingDictionary = fields.Where(f => f.Binding.BindingType != FieldBindingType.ActionButton).
                        ToDictionary(f => f.Binding.Key, f => f.Rules.Where(r => r.RuleTriggerType == parameters.TriggeredTriggerType));
                }

                bindingRowIndexResolution = SpreadBindingRulesForLists(bindingDictionary, parameters.Model);

                var triggeredFields = bindingDictionary.Keys.Where(k => bindingDictionary[k].Any() && k?.Contains(FieldBinding.ColumnIndexMarker) == false).ToList();

                if ((parameters.TriggeredTriggerType == FormRuleTriggers.ItemAdded || parameters.TriggeredTriggerType == FormRuleTriggers.ItemChanged
                    || parameters.TriggeredTriggerType == FormRuleTriggers.ItemDeleting) &&
                    triggeredFields.Count > parameters.RowIndex)
                {
                    //var rowField = triggeredFields.ElementAt(parameters.RowIndex);
                    var element = $"[{parameters.RowIndex}]";
                    var rowField = triggeredFields.FirstOrDefault(x => x.Contains(element));
                    triggeredFields = new List<string> { rowField };
                }

                if (triggeredFields.Count > 0)
                {
                    changedFields.PushRange(triggeredFields.ToArray());
                }
                // changedFields = new ConcurrentStack<string>(triggeredFields);
            }

            // Passing a lambda mutating changedFields
            //var modelProxy = _proxyProvider.GetModelProxy(parameters.Model, (jsonPath, prop, val) => changedFields.Push(jsonPath));

            if (!string.IsNullOrEmpty(parameters.TriggeredRuleCode))
            {
                var ruleDetails = _rulesDictionary[parameters.TriggeredRuleCode];
                ruleDetails.Instance.Initialize(parameters);
                ruleDetails.Instance.Result.Initialize(currentFieldsDisplayProperties);

                await ExecuteRule(changedFields, ruleDetails, parameters?.TriggeredFieldBinding?.Key, result, modelProxy, _logStreamer);
            }

            //else if (!string.IsNullOrEmpty(parameters.TriggeredFieldJsonPath))
            else if (parameters.TriggeredFieldBinding != null && parameters.TriggeredTriggerType == null)
            {
                // ToDo: if changedFields already contains "$.CardHistory[0]" we should not add another $.CardHistory[__index]
                changedFields.Push(parameters.TriggeredFieldBinding.Key);
            }

            // WARNING : changedFields is volatile!
            while (changedFields.TryPop(out var current))
            {
                if (!PropertyPathInDictionary(current, bindingDictionary))
                {
                    continue;
                }

                // check for infinite loop in rules for every iteration
                if (processedFileds.TryGetValue(current, out var invocationCount))
                {
                    if (invocationCount > 99)
                    {
                        throw new RuleExecutionInfiniteLoopException($"Infinite loop in rule: {current}");
                    }
                    else
                    {
                        processedFileds[current]++;
                    }
                }
                else
                {
                    processedFileds.Add(current, 1);
                }

                foreach (var rule in GetRulesForProperty(current, bindingDictionary, parameters.TriggeredTriggerType))
                {
                    parameters.AttachedFieldBinding = current;

                    if (bindingRowIndexResolution.ContainsKey(current))
                    {
                        parameters.RowIndex = bindingRowIndexResolution[current];
                    }

                    if (rule.RuleCode == null)
                    {
                        throw new Exception($"RuleCode is NULL for {rule.Name}");
                    }

                    var ruleDetails = _rulesDictionary[rule.RuleCode];
                    ruleDetails.Instance.Initialize(parameters);
                    ruleDetails.Instance.Result.Initialize(currentFieldsDisplayProperties);

                    await ExecuteRule(changedFields, ruleDetails, current, result, modelProxy, _logStreamer);
                }
            }

            // restore all models in hierarchy from proxies
            //var resultModel = _proxyProvider.GetProxyModel(modelProxy);
            result.Model = modelProxy;
            result.FieldsDisplayProperties = currentFieldsDisplayProperties;
            return result;
        }

        private IEnumerable<RuleDetails> GetRulesForProperty(string binding, Dictionary<string, IEnumerable<RuleDetails>> bindingDictionary,
            FormRuleTriggers? ruleTrigger)
        {
            var result = new List<RuleDetails>();
            var current = binding;
            CheckBinding(false);

            current = JsonPathHelper.RemoveLastProperty(current);
            CheckBinding(true);
            return result;

            void CheckBinding(bool parent)
            {
                if (bindingDictionary.ContainsKey(current))
                {
                    var rules = new List<RuleDetails>();

                    if (ruleTrigger != null)
                    {
                        rules = bindingDictionary[current].Where(r => r.RuleTriggerType == ruleTrigger).ToList();
                    }
                    else
                    {
                        rules = bindingDictionary[current].
                            Where(r => r.RuleTriggerType == FormRuleTriggers.Changed || r.RuleTriggerType == FormRuleTriggers.ItemChanged).ToList();
                    }

                    if (parent)
                    {
                        rules = rules.Where(r => r.IsOuterProperty).ToList();
                    }

                    result.AddRange(rules);
                }
            }
        }

        private bool PropertyPathInDictionary(string binding, Dictionary<string, IEnumerable<RuleDetails>> bindingDictionary)
        {
            var current = binding;

            if (bindingDictionary.ContainsKey(current))
            {
                return true;
            }

            current = JsonPathHelper.RemoveLastProperty(current);
            return bindingDictionary.ContainsKey(current);
        }

        private static async Task ExecuteRule(ConcurrentStack<string> changedFields, ExecutableRuleDetails ruleDetails, string affectedField, 
            RuleEngineExecutionResult result, object modelProxy, ILogStreamer logStreamer)
        {
            bool isAsync = ruleDetails.Instance.GetType().GetInterfaces()
                        .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IFlowRuleAsync<>));

            if (isAsync)
            {
                await ruleDetails.Instance.ExecuteUntypedAsync(modelProxy, logStreamer);
            }
            else
            {
                ruleDetails.Instance.ExecuteUntyped(modelProxy);
            }

            if (ruleDetails.Instance.Result.ValidationResult != RuleValidationResult.Ok)
            {
                ruleDetails.Instance.Result.AffectedField = affectedField;
                ruleDetails.Instance.Result.RuleCode = ruleDetails.RuleCode;
                result.Validations.Add(ruleDetails.Instance.Result);
            }

            if (ruleDetails.Instance.Result.SkipThisChange)
            {
                result.SkipThisChange = true;
            }

            if (ruleDetails.Instance.Result.ChangedFields.Any())
            {
                changedFields.PushRange(ruleDetails.Instance.Result.ChangedFields.ToArray());
            }
        }

        private Dictionary<string, int> SpreadBindingRulesForLists(Dictionary<string, IEnumerable<RuleDetails>> bindingDictionary, object model)
        {
            var bindingRowIndexResolution = new Dictionary<string, int>();
            var bindings = bindingDictionary.Keys.Where(k => k.Contains(RuleDefinitionParser.ColumnIndexMarker)).ToList();

            foreach (var binding in bindings)
            {
                var parts = binding.Split(RuleDefinitionParser.ColumnIndexMarker);
                var itemsBinding = parts[0];
                var itemPath = parts[1];
                var list = _navigator.GetItems(model, itemsBinding);

                if (list != null)
                {
                    var rules = bindingDictionary[binding];
                    // let's keep template binding for a while
                    //bindingDictionary.Remove(binding);

                    for (int i = 0; i < list.Count(); i++)
                    {
                        var newBinding = $"{itemsBinding}[{i}]{itemPath}";
                        bindingDictionary[newBinding] = rules;
                        bindingRowIndexResolution.Add(newBinding, i);
                    }
                }
            }

            return bindingRowIndexResolution;
        }
    }
}
