using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Proxyma;
using BlazorForms;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BlazorForms.Tests.Framework.Core;
using BlazorForms.Proxyma.Tests;
using BlazorForms.Platform.Tests.Models;

namespace BlazorForms.Platform.Tests.Rules
{
    public class DynamicRecordsetRuleTests : TestInfraAssemblyRegistratorBase
    {
        [Fact]
        public async Task DynamicRecordsetRuleExecutedTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };
            var q1Data = new ExpandoObject();
            q1Data.SetValue("Text", "some text");
            model.Ext["q1"] = new DynamicRecordset(q1Data);
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "DynamicRecordsetTestRule1", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.DynamicRecordsetTestFlow" };
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as Model2;

            Assert.Equal("Executed", (newModel.Ext["q1"].Data as dynamic).Message);
        }

        [Fact]
        public async Task DynamicRecordsetRuleExecutedAndTriggeredTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };
            var q1Data = new ExpandoObject();
            q1Data.SetValue("Text", "");
            model.Ext["q1"] = new DynamicRecordset(q1Data);
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "DynamicRecordsetTestRule2", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.DynamicRecordsetTestFlow" };
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as Model2;

            Assert.Equal("Text set", (newModel.Ext["q1"].Data as dynamic).Text);
            Assert.Equal("Triggered", (newModel.Ext["q1"].Data as dynamic).Output);
        }
    }

    public class DynamicRecordsetTestRule1 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "DynamicRecordsetTestRule1";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            dynamic q1 = model.Ext["q1"].Data;
            q1.Message = "Executed";
        }
    }

    public class DynamicRecordsetTestRule2 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "DynamicRecordsetTestRule2";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            dynamic q1 = model.Ext["q1"].Data;
            q1.Text = "Text set";
        }
    }

    public class DynamicRecordsetTestRule3 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "DynamicRecordsetTestRule3";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            var q1 = model.Ext["q1"].Data;
            q1.SetValue("Output", "Triggered");
        }
    }

    public class DynamicRecordsetTestFlow : FlowTaskDefinitionBase<Model2>
    {
        public object FirstName => ModelProp(m => m.Client.FirstName);

        [FlowRule(typeof(DynamicRecordsetTestRule3), FormRuleTriggers.Changed)]
        public object Text => CustomModelProp("$.Ext['q1'].Data.Text");
    }
}
