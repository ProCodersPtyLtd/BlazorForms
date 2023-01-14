using BlazorForms.FlowRules;
using BlazorForms.Shared;
using BlazorForms.Platform.ProcessFlow;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using BlazorForms.Shared.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using BlazorForms;
using BlazorForms.Proxyma;
using BlazorForms.Tests.Framework.Core;
using BlazorForms.Proxyma.Tests;

namespace BlazorForms.Platform.Tests.Rules
{
    public class ValidationRuleTests: TestInfraAssemblyRegistratorBase
    {
        [Fact]
        public async Task ValidationResultOkTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { };
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "rule1", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask3" };
            var execResult = await engine.Execute(parameters);
            Assert.Empty(execResult.Validations);
        }

        [Fact]
        public async Task ValidationResultWarningTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { };
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "rule2", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask3" };
            var execResult = await engine.Execute(parameters);
            Assert.NotEmpty(execResult.Validations);
            Assert.Equal(RuleValidationResult.Warning, execResult.Validations[0].ValidationResult);
            Assert.Equal("Should not be empty", execResult.Validations[0].ValidationMessage);
        }

        [Fact]
        public async Task ValidationResultErrorTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { };
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "rule3", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask3" };
            var execResult = await engine.Execute(parameters);
            Assert.NotEmpty(execResult.Validations);
            Assert.Equal(RuleValidationResult.Error, execResult.Validations[0].ValidationResult);
            Assert.Equal("Failed!", execResult.Validations[0].ValidationMessage);
        }

        [Fact]
        public async Task TriggerAllSubmitValidationRulesTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { };
            
            var parameters = new RuleExecutionParameters
            { Model = model, TriggeredTriggerType = FormRuleTriggers.Submit, 
                ProcessTaskTypeFullName = typeof(ProcessTask3).FullName };
            
            var execResult = await engine.Execute(parameters);
            Assert.NotEmpty(execResult.Validations);
            Assert.Equal(2, execResult.Validations.Count);
            Assert.Equal("$.Client.BirthDate", execResult.Validations.GroupBy(v => v.AffectedField).First().Key);
        }
    }

    public class ValidationRuleTestsRule1 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "rule1";
        public override RuleTypes RuleType => RuleTypes.ValidationRule;

        public override void Execute(Model2 model)
        {
            Result.ValidationResult = RuleValidationResult.Ok;
        }
    }

    public class ValidationRuleTestsRule2 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "rule2";
        public override RuleTypes RuleType => RuleTypes.ValidationRule;

        public override void Execute(Model2 model)
        {
            Result.ValidationResult = RuleValidationResult.Warning;
            Result.ValidationMessage = "Should not be empty";
        }
    }

    public class ValidationRuleTestsRule3 : FlowRuleBase<Model2>
    {
        public override string RuleCode => "rule3";
        public override RuleTypes RuleType => RuleTypes.ValidationRule;

        public override void Execute(Model2 model)
        {
            Result.ValidationResult = RuleValidationResult.Error;
            Result.ValidationMessage = "Failed!";
        }
    }

    public class ProcessTask3 : FlowTaskDefinitionBase<Model2>
    {
        [FlowRule(typeof(ValidationRuleTestsRule1), FormRuleTriggers.Submit)]
        [FlowRule(typeof(ValidationRuleTestsRule2), FormRuleTriggers.Submit)]
        [FlowRule(typeof(ValidationRuleTestsRule3), FormRuleTriggers.Submit)]
        public object BirthDate => ModelProp(e => e.Client.BirthDate);
    }
}
