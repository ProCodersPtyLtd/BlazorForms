using BlazorForms.Proxyma;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.FlowRules.Engine
{
    public class SimpleFastRuleEngine : RuleExecutionEngineBase
    {
        public SimpleFastRuleEngine(ILogStreamer logStreamer, IRuleDefinitionParser parser, IAssemblyRegistrator assemblyRegistrator, 
            IJsonPathNavigator navigator, IKnownTypesBinder binder)
            : base(logStreamer, parser, assemblyRegistrator, navigator, binder)
        {
        }

        public override async Task<RuleEngineExecutionResult> Execute(RuleExecutionParameters parameters)
        {
            return await base.Execute(parameters);
        }
    }
}
