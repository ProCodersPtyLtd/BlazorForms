using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Platform.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BlazorForms.Tests.Framework.Core;

namespace BlazorForms.Platform.Tests.FluentForms
{
    public class FluentFormsDefinitionTests 
    {
        private IFlowRunProvider _provider;
        private ServiceProvider _serviceProvider;

        public FluentFormsDefinitionTests()
        {
            var creator = new FlowRunProviderCreator();
            _provider = creator.GetFlowRunProvider();
            _serviceProvider = creator.ServiceProvider;
        }

        [Fact]
        public void SimpleFluentInvalidFormDefinitionTest()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                var form = new TestInvalidForm1() as IModelDefinitionForm;
                var fields = form.GetDetailsFields().ToList();

                Assert.Equal(3, fields.Where(f => f.Hidden == false).Count());
                Assert.Equal("$.Client.Id", fields[1].BindingProperty);
                Assert.Empty(fields.Where(f => f.ControlType == null));
                Assert.Empty(fields.Where(f => f.ViewModeControlType == null));
            });
        }

        [Fact]
        public void SimpleFluentFormDefinitionTest()
        {
            var form = new TestForm1() as IModelDefinitionForm;
            var fields = form.GetDetailsFields().ToList();

            // it should form global field and 3 controls, buttons ignored at this level
            Assert.Equal(4, fields.Where(f => f.Hidden == false).Count());
            Assert.Equal("$.Client.Id", fields[2].BindingProperty);
            Assert.Empty(fields.Where(f => f.ControlType == null && f.ControlTypeName == null));
            Assert.Empty(fields.Where(f => f.BindingProperty != ModelBinding.FormLevelBinding && f.ViewModeControlType == null));
        }

        [Fact]
        public void SimpleFluentFormControlTypeNameTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm1));
            var fields = details.Fields.ToList();

            var fid = fields.First(f => f.Binding.Binding == "$.Id");
            Assert.Equal("Label", fid.ControlType);
        }

        [Fact]
        public void SimpleFluentFormParseTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm1));
            Assert.True(details.Fields.Count > 0);
        }

        [Fact]
        public async Task SimpleFluentFormGetFormDetailsTest()
        {
            var form = await _provider.GetFormDetails(typeof(TestForm1).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.ClientId");

            Assert.Single(field.FlowRules);
            Assert.Equal("BlazorForms.Platform.Tests.FluentForms.TestForm1NameChangedRule", field.FlowRules[0].FormRuleCode);
            Assert.Equal("ClientId", form.Fields.Single(f => f.Binding.Binding == "$.Client.Id").Name);
            Assert.Equal("ClientId", form.Fields.Single(f => f.Binding.Binding == "$.Client.Id").Caption);
            Assert.True(form.Fields.Single(f => f.Binding.Binding == "$.Client.Id").DisplayProperties.Required);
        }

        [Fact]
        public void SimpleFluentFormButtonsParserTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm1));
            Assert.True(details.Fields.Count > 0);

            var cancelButton = details.Fields.FirstOrDefault(c => c.Name == "Cancel");
            Assert.Equal(ModelBinding.CloseButtonBinding, cancelButton?.Binding?.Binding);
            Assert.Equal(FieldBindingType.ActionButton, cancelButton?.Binding?.BindingType);
            Assert.Equal("Cancel", cancelButton?.DisplayProperties.Name);
            Assert.Equal("Cancel Me", cancelButton?.DisplayProperties.Caption);
            Assert.Equal(ModelBinding.CloseButtonBinding, cancelButton?.DisplayProperties?.Binding?.Binding);
            Assert.True(cancelButton?.DisplayProperties.Visible);

            var submitButton = details.Fields.FirstOrDefault(c => c.Name == "Submit");
            Assert.Equal(ModelBinding.SubmitButtonBinding, submitButton?.Binding?.Binding);
            Assert.Equal(FieldBindingType.ActionButton, submitButton?.Binding?.BindingType);
            Assert.Equal("Submit", submitButton?.DisplayProperties.Name);
            Assert.Equal("Submit", submitButton?.DisplayProperties.Caption);
            Assert.True(submitButton?.DisplayProperties.Visible);
        }

        [Fact]
        public void FluentFormListDefinitionTest()
        {
            var form = new TestFormList2() as IModelDefinitionForm;
            var fields = form.GetDetailsFields().ToList();

            Assert.True(fields.Any());
            var field = fields.Single(f => f.Binding.Binding == "$.CustomerId");
            Assert.Equal(FieldBindingType.TableColumn, field.Binding.BindingType);
            Assert.Equal(FieldBindingType.TableColumn, field.BindingType);
            Assert.Equal("$.Data[__index].CustomerId", field.Binding.TemplateKey);
        }

        [Fact]
        public void FluentFormListParserTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestFormList2));
            var fields = details.Fields;

            Assert.True(fields.Any());
            var field = fields.Single(f => f.Binding.Binding == "$.CustomerId");
            Assert.Equal(FieldBindingType.TableColumn, field.Binding.BindingType);
            Assert.Equal("$.Data[__index].CustomerId", field.Binding.TemplateKey);
        }

        [Fact]
        public async Task FluentFormListGetFormContextMenuTest()
        {
            var form = await _provider.GetFormDetails(typeof(TestFormList2).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.CustomerId");

            Assert.Equal("CustomerId", field.DisplayProperties.Caption);
            Assert.True(field.DisplayProperties.IsPrimaryKey);
            Assert.False(field.DisplayProperties.Visible);

            var menu = form.Fields.Single(f => f.Binding.BindingType == FieldBindingType.TableColumnContextMenu);
            Assert.Equal(4, menu.Binding.ContextMenuActions.Count);
            Assert.Equal("Details", menu.Binding.ContextMenuActions[0].Name);
            Assert.Equal("CustomerView/{0}", menu.Binding.ContextMenuActions[0].NavigationFormat);
        }

        [Fact]
        public async Task FluentFormListGetFormRefButtonsTest()
        {
            var form = await _provider.GetFormDetails(typeof(TestFormList2).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.CustomerId");

            var refs = form.Fields.Single(f => f.Binding.BindingType == FieldBindingType.FlowReferenceButtons);
            Assert.Equal(2, refs.Binding.ContextMenuActions.Count);
            Assert.Equal("Back", refs.Binding.ContextMenuActions[0].Name);
            Assert.Equal("FirstCustList", refs.Binding.ContextMenuActions[0].NavigationFormat);
        }

        [Fact]
        public async Task FluentFormDropdownTest()
        {
            var form = await _provider.GetFormDetails(typeof(TestForm3).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.CurrentClientId");
            
            Assert.Equal("DropDown", field.ControlType);
            Assert.Equal("ListBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.SingleSelect, field.Binding.BindingType);
            Assert.Equal("$.Clients", field.Binding.ItemsBinding);
            Assert.Equal("$.CurrentClientId", field.Binding.Key);
            Assert.Equal("$.Name", field.Binding.NameBinding);
            Assert.Equal("$.Id", field.Binding.IdBinding);
        }

        [Fact]
        public async Task FluentFormEditWithOptionsTest()
        {
            var form = await _provider.GetFormDetails(typeof(TestForm3).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.OrderItemId");

            Assert.Equal("Autocomplete", field.ControlType);
            Assert.Equal("ListBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.SingleField, field.Binding.BindingType);
            Assert.Equal("$.OrderItems", field.Binding.ItemsBinding);
            Assert.Equal("$.OrderItemId", field.Binding.Key);
            Assert.Equal("$.Id", field.Binding.NameBinding);
        }


        [Fact]
        public async Task AttributeFormGetFormDetailsTest()
        {
            var form = await _provider.GetFormDetails(typeof(AttributeTestForm1).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.ClientId");

            Assert.Single(field.FlowRules);
            Assert.Equal("BlazorForms.Platform.Tests.FluentForms.TestForm1NameChangedRule", field.FlowRules[0].FormRuleCode);
            Assert.Equal("NameClientId", form.Fields.Single(f => f.Binding.Binding == "$.ClientId").DisplayProperties.Caption);
            Assert.True(form.Fields.Single(f => f.Binding.Binding == "$.ClientId").DisplayProperties.Required);

            field = form.Fields.Single(f => f.Binding.Binding == "$.CurrentClientId");
            Assert.Equal("DropDown", field.ControlType);
            Assert.Equal("ListBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.SingleSelect, field.Binding.BindingType);
            Assert.Equal("$.Clients", field.Binding.ItemsBinding);
            Assert.Equal("$.CurrentClientId", field.Binding.Key);
            Assert.Equal("$.Name", field.Binding.NameBinding);
            Assert.Equal("$.Id", field.Binding.IdBinding);

            field = form.Fields.Single(f => f.Binding.Binding == "$.BaseCurrencySearch");
            Assert.Equal("Autocomplete", field.ControlType);
            Assert.Equal("ListBindingControlType", field.Binding.BindingControlType);
            Assert.Equal(FieldBindingType.SingleField, field.Binding.BindingType);
            Assert.Equal("$.OrderItems", field.Binding.ItemsBinding);
            Assert.Equal("$.BaseCurrencySearch", field.Binding.Key);
            Assert.Equal("$.ItemName", field.Binding.NameBinding);
        }

        [Fact]
        public async Task AttributeFormListGetFormDetailsTest()
        {
            var form = await _provider.GetFormDetails(typeof(ProjectListFlowForm1).FullName);
            var field = form.Fields.Single(f => f.Binding.Binding == "$.Id");

            Assert.Equal("Entity Id", field.DisplayProperties.Caption);
            Assert.True(field.DisplayProperties.IsPrimaryKey);
            Assert.False(field.DisplayProperties.Visible);

            var menu = form.Fields.Single(f => f.Binding.BindingType == FieldBindingType.TableColumnContextMenu);
            Assert.Equal(3, menu.Binding.ContextMenuActions.Count);
            Assert.Equal("Allocate Team", menu.Binding.ContextMenuActions[0].Name);
            Assert.Equal("/allocation/{0}", menu.Binding.ContextMenuActions[0].NavigationFormat);
        }
    }

    public class TestInvalidForm1 : FormEditBase<TestInvalidOrder>
    {
        protected override void Define(FormEntityTypeBuilder<TestInvalidOrder> f)
        {
            f.Property(p => p.Id).IsReadOnly();
            f.Property(p => p.Client.Id).IsRequired().IsReadOnly();
            f.Property(e => e.ClientId).Dropdown<TestClient>().Set(c => c.Id, c => c.Name).IsRequired().Label("Client").Rule(typeof(TestForm1NameChangedRule));

            f.Button("CustAddrCountList", ButtonActionTypes.Submit);
            f.Button("CustAddrCountList", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }

    public class TestForm1 : FormEditBase<TestOrder>
    {
        protected override void Define(FormEntityTypeBuilder<TestOrder> f)
        {
            f.Property(p => p.Id).IsReadOnly().Control(ControlType.Label);
            f.Property(p => p.Client.Id).IsRequired().IsReadOnly();
            f.Property(e => e.ClientId).Dropdown<TestClient>().Set(c => c.Id, c => c.Name).IsRequired().Label("Client").Rule(typeof(TestForm1NameChangedRule));

            f.Button("CustAddrCountList", ButtonActionTypes.Submit);
            f.Button("CustAddrCountList", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }

    public class TestForm3 : FormEditBase<TestOrder>
    {
        protected override void Define(FormEntityTypeBuilder<TestOrder> f)
        {
            f.Property(p => p.Id).IsReadOnly();
            f.Property(p => p.OrderItemId).EditWithOptions(m => m.OrderItems, m => m.Id).IsRequired();
            f.Property(p => p.CurrentClientId).Dropdown(p => p.Clients, c => c.Id, c => c.Name).IsRequired();
        }
    }

    public class TestForm1NameChangedRule : FlowRuleAsyncBase<TestOrder>
    {
        public override string RuleCode => this.GetType().FullName;

        public override async Task Execute(TestOrder model)
        {
            if (string.IsNullOrWhiteSpace(model.Client?.Name))
            {
                Result.ValidationResult = RuleValidationResult.Error;
                Result.ValidationMessage = "Please enter a valuable Client Name";
            }
        }
    }

    public class TestFormList2 : FormListBase<TestCustAddrCountModel>
    {
        protected override void Define(FormListBuilder<TestCustAddrCountModel> b)
        {
            b.List(p => p.Data, e =>
            {
                e.DisplayName = "Test Form List";
                e.Property(p => p.CustomerId).IsPrimaryKey().IsHidden();
                e.Property(p => p.FirstName).Label("Firts Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.LastName).Label("Last Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.AddrCount);
                e.Property(p => p.Phone);
                e.Property(p => p.EmailAddress);
                e.Property(p => p.CompanyName);

                e.ContextButton("Details", "CustomerView/{0}")
                    .ContextButton("Edit", "CustomerEdit/{0}")
                    .ContextButton("Addresses", "CustAddrList/{0}")
                    .ContextButton("Delete", "CustomerDelete/{0}");

                e.NavigationButton("Back", "FirstCustList", FlowReferenceOperation.Custom);
                e.NavigationButton("Next", "FinalList", FlowReferenceOperation.Custom);
            });
        }
    }

    [Form("Projects", ChildProcess = typeof(TestProjectListEditFlow))]
    public class ProjectListFlowForm1 : FlowTaskDefinitionBase<TestProjectListModel>
    {
        [Display("Entity Id", Visible = false, IsPrimaryKey = true)]
        public object EntityId => TableColumn(t => t.Projects, c => c.Id);

        [Display("Project Name")]
        public object CompanyName => TableColumn(t => t.Projects, c => c.Name);

        public object Menu => TableColumnContextMenu(t => t.Projects
            , new BindingFlowNavigationReference("Allocate Team", $"/allocation/{{0}}")
            , new BindingFlowReference("Edit", typeof(TestProjectListEditFlow), FlowReferenceOperation.Edit)
            //, ModelBindingFlowReference<ClientListModel>.FromModel("Add To", m => m.AddToActions)
            , new BindingFlowReference("Delete", typeof(TestProjectListEditFlow), FlowReferenceOperation.Delete)
        );

        public object RefButtons => FlowReferenceButtons(
            new BindingFlowReference("Add", typeof(TestProjectListEditFlow), FlowReferenceOperation.DialogForm)
        );
    }

    public class TestProjectListEditFlow
    { }

    [Form("Attribute Silent Form")]
    public class AttributeTestForm1 : FlowTaskDefinitionBase<TestOrder>
    {
        [FormComponent(typeof(TextEdit))]
        [Display("NameClientId", Required = true)]
        [FlowRule(typeof(TestForm1NameChangedRule), FormRuleTriggers.Changed)]
        public object NameControl => ModelProp(m => m.ClientId);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Initial Share Price")]
        public object SharePriceControl => ModelProp(m => m.Id);

        [FormComponent(typeof(DropDown))]
        [Display("Current Client")]
        public object CurrentClientControl => SingleSelect(t => t.CurrentClientId, p => p.Clients, m => m.Id, m => m.Name);

        [FormComponent(typeof(Autocomplete))]
        [Display("Base Currency")]
        public object ProjectBaseCurrencyControl => EditWithOptions(a => a.BaseCurrencySearch, e => e.OrderItems, m => m.ItemName);

        [Display("Cancel")]
        public object CancelButton => ActionButton(ActionType.Close);
        [Display("Submit")]
        public object SubmitButton => ActionButton(ActionType.Submit);
    }
}
