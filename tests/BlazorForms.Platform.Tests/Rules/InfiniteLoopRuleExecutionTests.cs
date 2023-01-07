using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Proxyma;
using BlazorForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BlazorForms.Tests.Framework.Core;
using BlazorForms.Proxyma.Tests;
using BlazorForms.Platform.Tests.Models;

namespace BlazorForms.Platform.Tests.Rules
{
    public class InfiniteLoopRuleExecutionTests : TestInfraAssemblyRegistratorBase
    {
        [Fact]
        public async Task RulesTriggerEachOtherTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "RuleSampleChangesBirthDate", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask5" };

            Exception ex = await Assert.ThrowsAsync<RuleExecutionInfiniteLoopException>(async() => await engine.Execute(parameters));
        }
    }

    public class RuleSampleChangesBirthDate : FlowRuleBase<Model2>
    {
        public override string RuleCode => "RuleSampleChangesBirthDate";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            model.Client.BirthDate = DateTime.Now;
        }
    }

    public class RuleSampleChangesCountry : FlowRuleBase<Model2>
    {
        public override string RuleCode => "RuleSampleChangesCountry";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            model.Client.ResidentialAddress.Country = "US";
        }
    }

    public class ProcessTask5 : FlowTaskDefinitionBase<Model2>
    {
        [FlowRule(typeof(RuleSampleChangesCountry), FormRuleTriggers.Changed)]
        public object BirthDate => ModelProp(e => e.Client.BirthDate);

        public object AddressPostCode => ModelProp(m => m.Client.ResidentialAddress.PostCode);

        [FlowRule(typeof(RuleSampleChangesBirthDate), FormRuleTriggers.Changed)]
        public object AddressCountry => ModelProp(m => m.Client.ResidentialAddress.Country);
    }
}
