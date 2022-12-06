using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Tests.Models;
using BlazorForms.Proxyma;
using BlazorForms.Proxyma.Tests;
using BlazorForms.Shared;
using BlazorForms.Tests.Framework.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorForms.Platform.Tests.Rules
{
    public class DisplayRuleTests : TestInfraAssemblyRegistratorBase
    {
        [Fact]
        public void DisplayRuleRunTest()
        {
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };
            var rule = new DisplayRuleTestsRule1();
            rule.Execute(model);
            var result = rule.Result;
            Assert.Equal(4, result.Fields.Dictionary.Keys.Count);
            Assert.True(result.Fields["$.Client.ResidentialAddress.StreetLine1"].Highlighted);
            Assert.NotEmpty(result.Fields["$.Client.ResidentialAddress.StreetLine1"].Hint);
            Assert.True(result.Fields["$.Client.ResidentialAddress.Country"].Highlighted);
            Assert.NotEmpty(result.Fields["$.Client.ResidentialAddress.Country"].Hint);
            Assert.True(result.Fields["$.Client.ResidentialAddress.State"].Highlighted);
            Assert.NotEmpty(result.Fields["$.Client.ResidentialAddress.State"].Hint);
            Assert.False(result.Fields["$.Client.ResidentialAddress.PostCode"].Required);
            Assert.False(result.Fields["$.Client.ResidentialAddress.PostCode"].Visible);
        }

        [Fact]
        public void DisplayRuleMergeResultTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };

            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "CheckBirthDateTestRule", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask4",
                FieldsDisplayProperties = new Dictionary<string, DisplayDetails>() };

            var execResult = engine.Execute(parameters);
            var displayProps = parameters.FieldsDisplayProperties;

            Assert.Equal(3, displayProps.Keys.Count);
            Assert.Equal("code", displayProps["$.Client.ResidentialAddress.PostCode"].Caption);
            Assert.Equal("mama mia", displayProps["$.Client.ResidentialAddress.PostCode"].Hint);
            Assert.True(displayProps["$.Client.ResidentialAddress.State"].Required);
        }

        [Fact]
        public void DisplayRuleMergeResultPresetDisplayPropertiesTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };

            var parameters = new RuleExecutionParameters
            {
                Model = model,
                TriggeredRuleCode = "CheckBirthDateTestRule",
                ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask4",
                FieldsDisplayProperties = new Dictionary<string, DisplayDetails>
                {
                    ["$.Client.ResidentialAddress.Country"] = new DisplayDetails { Caption = "Country" },
                }
            };

            var execResult = engine.Execute(parameters);
            var displayProps = parameters.FieldsDisplayProperties;

            Assert.Equal(3, displayProps.Keys.Count);
            Assert.Equal("Your Country", displayProps["$.Client.ResidentialAddress.Country"].Caption);
        }

        [Fact]
        public void DisplayRuleTriggeredFieldJsonPathTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };

            var parameters = new RuleExecutionParameters
            {
                Model = model,
                TriggeredRuleCode = "",
                TriggeredFieldBinding = new FieldBinding { Binding = "$.Client.BirthDate" },
                ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask4",
                FieldsDisplayProperties = new Dictionary<string, DisplayDetails>
                {
                    ["$.Client.ResidentialAddress.Country"] = new DisplayDetails { Caption = "Country" },
                }
            };

            engine.Execute(parameters);
        }

        [Fact]
        public async Task DisplayRuleEmptyTriggersRunTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };

            var parameters = new RuleExecutionParameters
            {
                Model = model,
                TriggeredRuleCode = "",
                TriggeredFieldBinding = null,
                TriggeredTriggerType = null,
                ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask4",
                FieldsDisplayProperties = new Dictionary<string, DisplayDetails>
                {
                    ["$.Client.ResidentialAddress.Country"] = new DisplayDetails { Caption = "Country" },
                }
            };

            Exception ex = await Assert.ThrowsAsync<ArgumentException>(async () => await engine.Execute(parameters));
        }
    }

    public class DisplayRuleTestsRule2 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "drule2";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            Result.Fields[SingleField(m => m.Client.ResidentialAddress.PostCode)].Caption = "code";
            Result.Fields[SingleField(m => m.Client.ResidentialAddress.State)].Required = true;

            if(Result.Fields[SingleField(m => m.Client.ResidentialAddress.Country)].Caption == "Country")
            {
                Result.Fields[SingleField(m => m.Client.ResidentialAddress.Country)].Caption = "Your Country";
            }
        }
    }

    public class DisplayRuleTestsRule3 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "drule3";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            Result.Fields[SingleField(m => m.Client.ResidentialAddress.PostCode)].Hint = "mama mia";
        }
    }

    public class CheckBirthDateTestRule : FlowRuleBase<Model2>
    {
        public override string RuleCode => "CheckBirthDateTestRule";
        public override RuleTypes RuleType => RuleTypes.ValidationRule;

        public override void Execute(Model2 model)
        {
            // trigger PostCode changed rules
            model.Client.ResidentialAddress.PostCode = "9999";
        }
    }

    public class ProcessTask4 : FlowTaskDefinitionBase<Model2>
    {
        [FlowRule(typeof(CheckBirthDateTestRule), FormRuleTriggers.Changed)]
        public object BirthDate => ModelProp(e => e.Client.BirthDate);

        [FlowRule(typeof(DisplayRuleTestsRule2), FormRuleTriggers.Changed)]
        [FlowRule(typeof(DisplayRuleTestsRule3), FormRuleTriggers.Changed)]
        public object AddressPostCode => ModelProp(m => m.Client.ResidentialAddress.PostCode);
    }

    public class DisplayRuleTestsRule1 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "drule1";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            var path = SingleField(m => m.Client.ResidentialAddress.StreetLine1);
            Result.Fields[path].Highlighted = true;
            Result.Fields[path].Hint = "Attention to this field";

            Result.Fields[SingleField(m => m.Client.ResidentialAddress.State)].Set(f =>
            {
                f.Hint = "Attention to this field";
                f.Highlighted = true;
            });

            Result.Fields[SingleField(m => m.Client.ResidentialAddress.Country)].Set(f => f.Highlighted = true).Set(f => f.Hint = "Attention to this field");

            var field = Result.Fields[SingleField(m => m.Client.ResidentialAddress.PostCode)];
            field.Required = false;
            field.Visible = false;

        }
    }
}
