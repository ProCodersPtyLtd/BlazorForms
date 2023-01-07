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
using BlazorForms.Proxyma;
using BlazorForms.Tests.Framework.Core;
using BlazorForms.Proxyma.Tests;
using BlazorForms.Platform.Tests.Models;

namespace BlazorForms.Platform.Tests.Rules
{
    public class RuleExecutionTests : TestInfraAssemblyRegistratorBase
    {
        private IFlowRunProvider _flowRunProvider;

        public RuleExecutionTests()
        {
            _flowRunProvider = new FlowRunProviderCreator().GetFlowRunProvider();
        }

        [Fact]
        public async Task JsonPathBasedExecutionTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now ,ResidentialAddress = new AddressModel { PostCode = "2227", Country = "AU" } } };
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "sample1", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask2" };
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as Model2;

            Assert.Equal("0000", newModel.Client.ResidentialAddress.PostCode);
            Assert.Equal("US", newModel.Client.ResidentialAddress.Country);

            newModel.Client.ResidentialAddress.PostCode = "1111";
            newModel.Client.ResidentialAddress.Country = "NZ";
            Assert.Equal("1111", newModel.Client.ResidentialAddress.PostCode);
            Assert.Equal("NZ", newModel.Client.ResidentialAddress.Country);

            parameters = new RuleExecutionParameters { Model = newModel, TriggeredRuleCode = "sample1", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask2" };
            execResult = await engine.Execute(parameters);
            var newModel2 = execResult.Model as Model2;
            Assert.Equal("0000", newModel2.Client.ResidentialAddress.PostCode);
            Assert.Equal("US", newModel2.Client.ResidentialAddress.Country);
        }

        [Fact]
        public async Task AsyncRuleTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new Model2 { Client = new ClientModel { BirthDate = DateTime.Now, ClientId = 14, FirstName = "lala" } };
            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "sampleAsync1", ProcessTaskTypeFullName = "BlazorForms.Platform.Tests.Rules.ProcessTask2" };
            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as Model2;

            Assert.Equal(newModel.Client.ClientId.ToString(), newModel.Client.FirstName);
        }

        [Fact]
        public async Task MoneyCascadingRuleTest()
        {
            var gen = new ProxyGenerator();
            var pa = new RuleDefinitionParser(_serviceProvider);
            var pr = new ModelProxyFactoryProvider(_proxyScopeConfiguration, gen);
            var engine = new InterceptorBasedRuleEngine(pa, this, pr, new JsonPathNavigator(), _serviceProvider.GetRequiredService<ILogStreamer>(), _serviceProvider.GetRequiredService<IKnownTypesBinder>());
            var model = new CascadingMoneyModel { Data = new CascadingMoney { Price = new Money() } };
            model.Data.Price.Amount = 100m;
            model.Data.Price.Currency = "ETH";
            var form = await _flowRunProvider.GetFormDetails(typeof(CascadingMoneyForm).FullName);

            var ruleFields = form.Fields.Select(f => new BlazorForms.FlowRules.FieldDetails
            {
                Binding = f.Binding,
                Rules = new Collection<RuleDetails>(f.FlowRules.Select(r => new RuleDetails
                {
                    FullName = r.FormRuleType,
                    RuleCode = r.FormRuleCode,
                    RuleTriggerType = Enum.Parse<FormRuleTriggers>(r.FormRuleTriggerType),
                    IsOuterProperty = r.IsOuterProperty
                }).ToList())
            });

            var parameters = new RuleExecutionParameters { Model = model, TriggeredRuleCode = "CascadingMoneyRule1", 
                Fields = new Collection<BlazorForms.FlowRules.FieldDetails>(ruleFields.ToList()),
                ProcessTaskTypeFullName = typeof(CascadingMoneyForm).FullName };

            // ToDo: sometimes it fails
            Assert.True(_proxyScopeConfiguration.GetScopeTypes().Contains(typeof(Money)), "Money should be in the proxy scope");

            var execResult = await engine.Execute(parameters);
            var newModel = execResult.Model as CascadingMoneyModel;

            Assert.Equal(0, newModel.Data.Price.Amount);
            // CascadingMoneyRule2 should be triggered by changes made to model in CascadingMoneyRule1
            Assert.Equal("Empty", newModel.Status);
        }
    }

    #region Cascading Money rules
    [ProxyScope]
    public class CascadingMoneyModel : FlowModelBase
    {
        public virtual CascadingMoney Data { get; set; }
        public virtual string Status { get; set; }
    }

    [ProxyScope]
    public class CascadingMoney : FlowModelBase
    {
        public virtual Money Price { get; set; }
    }

    public class CascadingMoneyForm : FormEditBase<CascadingMoneyModel>
    {
        protected override void Define(FormEntityTypeBuilder<CascadingMoneyModel> f)
        {
            f.DisplayName = "Project Settings - Fluent Form";

            f.Property(p => p.Data.Price).Control(typeof(MoneyEdit))
                .Rule(typeof(CascadingMoneyRule2), isOuterProperty: true);
        }
    }

    public class CascadingMoneyRule1 : FlowRuleAsyncBase<CascadingMoneyModel>
    {
        public override string RuleCode => "CascadingMoneyRule1";

        public override async Task Execute(CascadingMoneyModel model)
        {
            model.Data.Price.Amount = 0;
            Result.ValidationResult = RuleValidationResult.Ok;
        }
    }

    public class CascadingMoneyRule2 : FlowRuleAsyncBase<CascadingMoneyModel>
    {
        public override string RuleCode => "CascadingMoneyRule2";

        public override async Task Execute(CascadingMoneyModel model)
        {
            if (model.Data.Price.Amount == 0)
            {
                model.Status = "Empty";
            }

            Result.ValidationResult = RuleValidationResult.Ok;
        }
    }
    #endregion


    #region Repeater Rule Classes

    public class RepeaterRowRule1 : FlowRuleBase<RepeaterRowModel>
    {
        public override string RuleCode => "RepeaterRowRule1";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(RepeaterRowModel model)
        {
            var record = model.Rows[RunParams.RowIndex];
            record.IsSubscriptionAdmin = false;
        }
    }

    public class RepeaterRowRule2 : FlowRuleBase<RepeaterRowModel>
    {
        public override string RuleCode => "RepeaterRowRule2";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(RepeaterRowModel model)
        {
            // changing Login should trigger Login atteched rules
            model.Rows[RunParams.RowIndex].Login = "lala";
        }
    }

    public class RepeaterRowRule3 : FlowRuleBase<RepeaterRowModel>
    {
        public override string RuleCode => "RepeaterRowRule3";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(RepeaterRowModel model)
        {
            // default rule
            model.Rows[RunParams.RowIndex].MembershipTypeCode = "Basic";
        }
    }

    public class RepeaterRowRule4 : FlowRuleBase<RepeaterRowModel>
    {
        public override string RuleCode => "RepeaterRowRule4";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(RepeaterRowModel model)
        {
            Result.ValidationResult = RuleValidationResult.Warning;
            Result.ValidationMessage = "Show Warning always";
        }
    }

    public class RepeaterRowRule5 : FlowRuleBase<RepeaterRowModel>
    {
        public override string RuleCode => "RepeaterRowRule5";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(RepeaterRowModel model)
        {
            // type safe notation
            Result.Fields[RowField(m => m.Rows, i => i.Login, RunParams.RowIndex)].Hint = "mama mia";
            // another notation
            Result.Fields[$"$.Rows[{RunParams.RowIndex}].Login"].Hint = "mama mia";
        }
    }

    public class RepeaterRowTask1 : FlowTaskDefinitionBase<RepeaterRowModel>
    {
        [FlowRule(typeof(RepeaterRowRule3), FormRuleTriggers.ItemAdded)]
        [FormComponent(typeof(Repeater))]
        [Display("Subscription members")]
        public IFieldBinding Rows => Repeater(t => t.Rows);

        [FlowRule(typeof(RepeaterRowRule1), FormRuleTriggers.Changed)]
        [FlowRule(typeof(RepeaterRowRule5), FormRuleTriggers.Changed)]
        [FormComponent(typeof(TextEdit))]
        [Display("Login", Required = true)]
        public IFieldBinding LoginControl => TableColumn(t => t.Rows, m => m.Login);

        [FlowRule(typeof(RepeaterRowRule2), FormRuleTriggers.Changed)]
        [FormComponent(typeof(TextEdit))]
        [Display("Access Type", Disabled = true)]
        public IFieldBinding MembershipTypeCode => TableColumn(t => t.Rows, m => m.MembershipTypeCode);

        [FlowRule(typeof(RepeaterRowRule4), FormRuleTriggers.Changed)]
        [FormComponent(typeof(Checkbox))]
        [Display("Is Subscription Admin")]
        public IFieldBinding IsSubscriptionAdmin => TableColumn(t => t.Rows, m => m.IsSubscriptionAdmin);
    }

    [ProxyScope]
    public class RepeaterRowDetails
    {
        public virtual string Login { get; set; }
        public virtual string MemberName { get; set; }
        public virtual DateTime? Created { get; set; }
        public virtual bool IsSubscriptionAdmin { get; set; }
        public virtual string MembershipTypeCode { get; set; }
        public virtual bool Modified { get; set; }
    }

    public class RepeaterRowModel : FlowModelBase
    {
        public virtual string Name { get; set; }

        public virtual List<RepeaterRowDetails> Rows { get; set; }
    }

    #endregion

    public class RuleSampleDate : FlowRuleBase<Model2>
    {
        public override string RuleCode => "sample1";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            model.Client.ResidentialAddress.PostCode = "0000";
            Result.ValidationResult = RuleValidationResult.Ok;
        }
    }

    public class RuleSamplePostCode : FlowRuleBase<Model2>
    {
        public override string RuleCode => "sample2";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override void Execute(Model2 model)
        {
            model.Client.ResidentialAddress.Country = "US";
        }
    }

    public class ClientIdExistsAsyncRule : FlowRuleAsyncBase<Model2>
    {
        public override string RuleCode => "sampleAsync1";
        public override RuleTypes RuleType => RuleTypes.DisplayRule;

        public override async Task Execute(Model2 model)
        {
            var helper = new TestAsyncHelper();
            int val = await helper.SlowReadingInt(model.Client.ClientId.Value);
            model.Client.FirstName = val.ToString();
        }
    }

    public class ProcessTask2 : FlowTaskDefinitionBase<Model2>
    {
        [FlowRule(typeof(RuleSampleDate), FormRuleTriggers.Changed)]
        public object BirthDate => ModelProp(e => e.Client.BirthDate);

        [FlowRule(typeof(RuleSamplePostCode), FormRuleTriggers.Changed)]
        public object AddressPostCode => ModelProp(m => m.Client.ResidentialAddress.PostCode);

        public object AddressCountry => ModelProp(m => m.Client.ResidentialAddress.Country);

        [FlowRule(typeof(ClientIdExistsAsyncRule), FormRuleTriggers.Changed)]
        public object ClientId => ModelProp(m => m.Client.ClientId);

        public object FirstName => ModelProp(m => m.Client.FirstName);
    }

    public class TestAsyncHelper
    {
        public async Task<int> SlowReadingInt(int val)
        {
            var result = 0;

            await Task.Run(async () => 
            {
                await Task.Delay(150);
                result = val;
            });

            return result;
        }
    }
}
