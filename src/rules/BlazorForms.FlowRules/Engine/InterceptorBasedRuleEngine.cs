using BlazorForms.FlowRules;
using BlazorForms.FlowRules.Engine;
using BlazorForms.Proxyma;
using BlazorForms.Shared;
using BlazorForms.Shared.Exceptions;
using BlazorForms.Shared.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules
{
    public class InterceptorBasedRuleEngine : RuleExecutionEngineBase
    {
        private readonly IJsonPathNavigator _navigator;
        private readonly IEnumerable<Assembly> _asms;
        private readonly Dictionary<string, Type> _allTypes;
        private readonly IEnumerable<ExecutableRuleDetails> _allRules;
        private readonly Dictionary<string, ExecutableRuleDetails> _rulesDictionary;
        private readonly IRuleDefinitionParser _parser;
        private readonly IAssemblyRegistrator _assemblyRegistrator;
        private readonly IProxymaProvider _proxyProvider;
        private readonly ILogStreamer _logStreamer;
        private readonly IKnownTypesBinder _binder;

        public InterceptorBasedRuleEngine(IRuleDefinitionParser parser, IAssemblyRegistrator assemblyRegistrator, IProxymaProvider proxyProvider,
            IJsonPathNavigator navigator, ILogStreamer logStreamer, IKnownTypesBinder binder)
            : base(logStreamer, parser, assemblyRegistrator, navigator, binder)
        {
            _proxyProvider = proxyProvider;
        }

        public override async Task<RuleEngineExecutionResult> Execute(RuleExecutionParameters parameters)
        {
            var changedFields = new ConcurrentStack<string>();
            var modelProxy = _proxyProvider.GetModelProxy(parameters.Model, (jsonPath, prop, val) => changedFields.Push(jsonPath));
            var result = await ExecuteLogic(parameters, modelProxy, changedFields);
            var resultModel = _proxyProvider.GetProxyModel(modelProxy);
            result.Model = resultModel;
            return result;
        }
    }
}
