using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Proxyma;
using BlazorForms.Platform.Shared.Attributes;
using BlazorForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BlazorForms.Tests.Framework.Core;

namespace BlazorForms.Platform.Tests.Rules
{
    public class RepeaterRuleExecutionTests : TestInfraAssemblyRegistratorBase
    {
        private IFlowRunProvider _flowRunProvider;

        public RepeaterRuleExecutionTests()
        {
            _flowRunProvider = new FlowRunProviderCreator().GetFlowRunProvider();
        }

        [Fact]
        public async Task RepeaterRowRuleNoCascadingLoopTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true });
            
            var parameters = new RuleExecutionParameters
            { RowIndex = 1, Model = model, TriggeredRuleCode = typeof(RepeaterRowChangedTestRule).Name, 
                ProcessTaskTypeFullName = typeof(RepeaterRowTestTask1).FullName, TriggeredTriggerType = FormRuleTriggers.ItemChanged };
            
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as RepeaterRowModel;

            Assert.True(newModel.Rows[1].Modified);
        }

        [Fact]
        public async Task RepeaterRowCascadingRuleTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true });

            var parameters = new RuleExecutionParameters
            {
                RowIndex = 1,
                Model = model,
                TriggeredRuleCode = typeof(RepeaterRowRule2).Name,
                ProcessTaskTypeFullName = typeof(RepeaterRowTask1).FullName
            };

            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as RepeaterRowModel;

            Assert.True(newModel.Rows[0].IsSubscriptionAdmin);
            Assert.False(newModel.Rows[1].IsSubscriptionAdmin);
        }

        [Fact]
        public async Task RepeaterRowRuleWithBindingReturnsDisplayPropertyTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());

            int rowIndex = 1;
            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });

            var binding = new FieldBinding { Binding = "$.Login", TableBinding = "$.Rows" };
            binding.ResolveKey(new FieldBindingArgs { RowIndex = rowIndex });

            var parameters = new RuleExecutionParameters
            {
                RowIndex = rowIndex,
                Model = model,
                TriggeredFieldBinding = binding,
                //TriggeredRuleCode = "RepeaterRowRule4", 
                ProcessTaskTypeFullName = typeof(RepeaterRowTask1).FullName,

                FieldsDisplayProperties = new Dictionary<string, DisplayDetails>
                {
                    [$"$.Rows[{rowIndex}].Login"] = new DisplayDetails { Hint = "enter here" },
                }
            };


            var execResult = await engine.Execute(parameters);
            var displayProps = parameters.FieldsDisplayProperties;

            Assert.Single(displayProps.Keys);
            Assert.Equal("mama mia", displayProps[$"$.Rows[{rowIndex}].Login"].Hint);
        }

        [Fact]
        public async Task RepeaterRowRuleWithBindingReturnsValidationErrorTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());

            int rowIndex = 1;
            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });

            var binding = new FieldBinding { Binding = "$.IsSubscriptionAdmin", TableBinding = "$.Rows" };
            binding.ResolveKey(new FieldBindingArgs { RowIndex = rowIndex });

            var parameters = new RuleExecutionParameters { RowIndex = rowIndex, Model = model, TriggeredFieldBinding = binding, TriggeredRuleCode = "RepeaterRowRule4", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.RepeaterRowTask1" };
            var execResult = await engine.Execute(parameters);
            var validations = execResult.Validations;

            Assert.Single(validations);
            Assert.Equal("$.Rows[1].IsSubscriptionAdmin", validations[0].AffectedField);
        }

        [Fact]
        public async Task RepeaterRowRuleReturnsValidationErrorTest()
        {
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, _proxyGenerator);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());

            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });

            var parameters = new RuleExecutionParameters { RowIndex = 1, Model = model, TriggeredRuleCode = "RepeaterRowRule4", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.RepeaterRowTask1" };
            var execResult = await engine.Execute(parameters);
            var validations = execResult.Validations;

            Assert.Single(validations);
            Assert.Null(validations[0].AffectedField);
        }

        [Fact]
        public async Task RepeaterRowItemAddedRuleTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true, MembershipTypeCode = "" });
            var parameters = new RuleExecutionParameters { RowIndex = 1, Model = model, TriggeredRuleCode = "RepeaterRowRule3", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.RepeaterRowTask1" };
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as RepeaterRowModel;

            Assert.Equal("", newModel.Rows[0].MembershipTypeCode);
            Assert.Equal("Basic", newModel.Rows[1].MembershipTypeCode);
        }

        [Fact]
        public async Task RepeaterRowRuleTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>());
            var model = new RepeaterRowModel { Rows = new List<RepeaterRowDetails>() };
            model.Rows.Add(new RepeaterRowDetails { Login = "a@b.com", IsSubscriptionAdmin = true });
            model.Rows.Add(new RepeaterRowDetails { Login = "a1@b1.com", IsSubscriptionAdmin = true });
            var parameters = new RuleExecutionParameters { RowIndex = 1, Model = model, TriggeredRuleCode = "RepeaterRowRule1", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.RepeaterRowTask1" };
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as RepeaterRowModel;

            Assert.True(newModel.Rows[0].IsSubscriptionAdmin);
            Assert.False(newModel.Rows[1].IsSubscriptionAdmin);
        }


    }

    #region Repeater Rule Classes

    public class RepeaterRowChangedTestRule : FlowRuleBase<RepeaterRowModel>
    {
        public override string RuleCode => GetType().Name;
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(RepeaterRowModel model)
        {
            model.Rows[RunParams.RowIndex].Modified = true;
        }
    }

    public class RepeaterRowTestTask1 : FlowTaskDefinitionBase<RepeaterRowModel>
    {
        [FlowRule(typeof(RepeaterRowChangedTestRule), FormRuleTriggers.ItemChanged)]
        [FormComponent(typeof(Repeater))]
        [Display("Subscription members")]
        public IFieldBinding Rows => Repeater(t => t.Rows);

        [FormComponent(typeof(TextEdit))]
        [Display("Login", Required = true)]
        public IFieldBinding LoginControl => TableColumn(t => t.Rows, m => m.Login);

        [FormComponent(typeof(TextEdit))]
        [Display("Access Type", Disabled = true)]
        public IFieldBinding MembershipTypeCode => TableColumn(t => t.Rows, m => m.MembershipTypeCode);

        [FormComponent(typeof(Checkbox))]
        [Display("Is Subscription Admin")]
        public IFieldBinding IsSubscriptionAdmin => TableColumn(t => t.Rows, m => m.IsSubscriptionAdmin);
    }


    #endregion

}
