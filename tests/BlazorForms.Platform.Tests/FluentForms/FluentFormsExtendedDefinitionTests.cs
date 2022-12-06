using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform;
using BlazorForms.Platform.ProcessFlow;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BlazorForms.Tests.Framework.Core;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Platform.Tests.FluentForms
{
    public class FluentFormsExtendedDefinitionTests
    {
        private IFlowRunProvider _provider;
        private ServiceProvider _serviceProvider;

        public FluentFormsExtendedDefinitionTests()
        {
            var creator = new FlowRunProviderCreator();
            _provider = creator.GetFlowRunProvider();
            _serviceProvider = creator.ServiceProvider;
        }
 
        [Fact]
        public void FluentFormRepeaterParseTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var form = parser.Parse(typeof(TestRepeaterFormEdit1));

            Assert.True(form.Fields.Count > 0);

            var field = form.Fields.Single(f => f.Binding.Key == "$.Roles[__index]");
            Assert.Single(field.FlowRules);
            Assert.Equal("RepeaterBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.Repeater, field.Binding.BindingType);
            Assert.Equal("Repeater", field.ControlType);

            var f1 = form.Fields.Single(f => f.Binding.Key == "$.Roles[__index].Name");
            Assert.Equal(FieldBindingType.TableColumn, f1.Binding.BindingType);
            Assert.Equal("Role", f1.DisplayProperties.Caption);
            Assert.Equal("TextEdit", f1.ControlType);

            var f2 = form.Fields.Single(f => f.Binding.Binding == "$.HourlyRate");
            Assert.Equal("Hourly Rate", f2.DisplayProperties.Caption);
            Assert.Equal("MoneyEdit", f2.ControlType);
        }

        [Fact]
        public void FluentFormTableParseTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var form = parser.Parse(typeof(TestRepeaterFormEdit1));
            Assert.True(form.Fields.Count > 0);

            var field = form.Fields.Single(f => f.Binding.Key == "$.CurrencyListRef[__index]");
            Assert.Single(field.FlowRules);
            Assert.Equal("TableBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.Table, field.Binding.BindingType);

            var f2 = form.Fields.Single(f => f.Binding.Binding == "$.ShortName");
            Assert.Equal("Short Name", f2.Caption);
        }

        [Fact]
        public async Task AttributeFormRepeaterParseTest()
        {
            var form = await _provider.GetFormDetails(typeof(TestAttributeRepeaterFormEdit1).FullName);
            Assert.True(form.Fields.Count > 0);

            var field = form.Fields.Single(f => f.Binding.Key == "$.Roles[__index]");
            Assert.Single(field.FlowRules);
            Assert.Equal("RepeaterBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.Repeater, field.Binding.BindingType);
            Assert.Equal("Repeater", field.ControlType);

            var f1 = form.Fields.Single(f => f.Binding.Key == "$.Roles[__index].Name");
            Assert.Equal("Role", f1.DisplayProperties.Caption);
            Assert.Equal("TextEdit", f1.ControlType);

            var f2 = form.Fields.Single(f => f.Binding.Binding == "$.HourlyRate");
            Assert.Equal("Hourly Rate", f2.DisplayProperties.Caption);
            Assert.Equal("MoneyEdit", f2.ControlType);
        }
    }

    public class TestRepeaterFormEdit1AddedRule : FlowRuleAsyncBase<TestArtelProjectSettingsModel>
    {
        public override string RuleCode => "TR85";

        public override async Task Execute(TestArtelProjectSettingsModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Project?.Name))
            {
                Result.ValidationResult = RuleValidationResult.Error;
                Result.ValidationMessage = "Project name cannot be empty";
            }
        }
    }

    public class TestRepeaterFormEdit1 : FormEditBase<TestArtelProjectSettingsModel>
    {
        protected override void Define(FormEntityTypeBuilder<TestArtelProjectSettingsModel> f)
        {
            f.Property(p => p.Project.Id).IsPrimaryKey().IsReadOnly();
            f.Property(p => p.Project.Name);

            f.Repeater(p => p.Roles, e =>
            {
                e.DisplayName = "Project Roles";
                e.Property(p => p.Name).Label("Role");
                e.Property(p => p.HourlyRate).Label("Hourly Rate").Control(typeof(MoneyEdit));

            }).Rule(typeof(TestRepeaterFormEdit1AddedRule), FormRuleTriggers.ItemAdded);

            f.Table(p => p.CurrencyListRef, e =>
            {
                e.DisplayName = "Currencies";
                e.Property(p => p.ShortName).Label("Short Name");
                e.Property(p => p.Name);

            }).Rule(typeof(TestRepeaterFormEdit1AddedRule), FormRuleTriggers.ItemAdded);
        }
    }

    public class TestAttributeRepeaterFormEdit1 : FlowTaskDefinitionBase<TestArtelProjectSettingsModel>
    {
        // Roles table
        [FlowRule(typeof(TestRepeaterFormEdit1AddedRule), FormRuleTriggers.ItemAdded)]
        [FormComponent(typeof(Repeater))]
        [Display("Project Roles")]
        public IFieldBinding Members => Repeater(t => t.Roles);

        [FormComponent(typeof(TextEdit))]
        [Display("Role", Required = true)]
        //[FlowRule(typeof(ProjectRoleNameEmptyRule), RuleTriggerTypes.Changed)]
        //[FlowRule(typeof(ProjectRoleNameEmptyRule), RuleTriggerTypes.ItemChanged)]
        public IFieldBinding RoleControl => TableColumn(t => t.Roles, m => m.Name);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Hourly Rate", Required = true)]
        public IFieldBinding RateControl => TableColumn(t => t.Roles, m => m.HourlyRate);
    }

    public class TestArtelProjectSettingsModel : FlowModelBase
    {
        public virtual string Message { get; set; }
        public virtual TestArtelProjectDetails Project { get; set; }

        public virtual List<TestArtelRoleDetails> Roles { get; set; }

        public virtual List<Currency> CurrencyListRef { get; set; }
        public virtual List<TestFrequencyTypeDetails> FrequencyRef { get; set; }
    }

    public class TestArtelRoleDetails
    {
        public virtual int Id { get; set; }
        public virtual int ArtelProjectId { get; set; }
        public virtual string Name { get; set; }
        public virtual Money HourlyRate { get; set; } = new Money();
    }

    public class TestFrequencyTypeDetails
    {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
    }

    public class TestArtelProjectDetails
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual TestArtelProjectStatistics Statistics { get; set; }
        public virtual string BaseCurrencySearch { get; set; }
        public virtual decimal DefaultSharesPaymentProportionPercent { get; set; }
        public virtual Money InitialSharePrice { get; set; } = new Money();
        public virtual string PaymentFrequencyCode { get; set; }
        public virtual int PaymentFrequencyDay { get; set; }
    }

    public class TestArtelProjectStatistics
    {
        public virtual Money TotalValue { get; set; } = new Money();
        public virtual int MemberCount { get; set; }
        public virtual int TotalSharesIssued { get; set; }
        public virtual int TotalTimesheetHours { get; set; }
        public virtual Money CurrentBalance { get; set; } = new Money();
    }
}
