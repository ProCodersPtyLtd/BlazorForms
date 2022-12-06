using BlazorForms.Proxyma;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules
{
    public class SimpleRuleEngine
    {
        private readonly IJsonPathNavigator _navigator;
        private readonly IEnumerable<Assembly> _asms;
        private readonly Dictionary<string, Type> _allTypes;
        private readonly IEnumerable<ExecutableRuleDetails> _allRules;
        private readonly Dictionary<string, ExecutableRuleDetails> _rulesDictionary;
        private readonly IRuleDefinitionParser _parser;
        private readonly IAssemblyRegistrator _assemblyRegistrator;
        private readonly IProxymaProvider _proxyProvider;
        private ILogStreamer _logStreamer;
        //private readonly List<string> _changedFields = new List<string>();

        // ToDo: should be refactored to load only simple rules
        public SimpleRuleEngine(IRuleDefinitionParser parser, IAssemblyRegistrator assemblyRegistrator, IProxymaProvider proxyProvider,
            IJsonPathNavigator navigator)
        {
            _parser = parser;
            _assemblyRegistrator = assemblyRegistrator;
            _proxyProvider = proxyProvider;
            _navigator = navigator;
            _asms = _assemblyRegistrator.GetConsideredAssemblies();
            _allTypes = _asms
                            .SelectMany(a => a.GetTypes())
                            .GroupBy(t => t.FullName)
                            .ToDictionary(t => t.First().FullName, t => t.First());

            _allRules = _parser.FindAllRules(_asms);
            _rulesDictionary = _allRules.ToDictionary(d => d.RuleCode, d => d);
        }

        public async Task<RuleEngineExecutionResult> Execute(RuleExecutionParameters parameters)
        {
            var rule = _rulesDictionary[parameters.TriggeredRuleCode];
            var accessRule = rule.Instance as IAccessRuleAsyncBase;
            accessRule.AccessModel = parameters.AccessModel as AccessRuleModel;
            var result = new RuleEngineExecutionResult { Model = parameters.Model, AccessModel = accessRule.AccessModel };
            await ExecuteRule(rule, "", result, parameters.Model, _logStreamer);
            return result;
        }

        private static async Task ExecuteRule(ExecutableRuleDetails ruleDetails, string affectedField, RuleEngineExecutionResult result, object modelProxy, ILogStreamer logStreamer)
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
        }
    }
}
