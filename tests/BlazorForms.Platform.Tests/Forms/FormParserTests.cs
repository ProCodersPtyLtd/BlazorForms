using Microsoft.Extensions.DependencyInjection;
using BlazorForms.Flows.Definitions;
using BlazorForms.FlowRules;
using BlazorForms.Shared;
using BlazorForms.Platform.ProcessFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using BlazorForms.Forms;
using BlazorForms.Tests.Framework.Core;

namespace BlazorForms.Platform.Tests.Forms
{
    public class FormParserTests
    {
        private IFlowRunProvider _provider;
        private ServiceProvider _serviceProvider;

        public FormParserTests()
        {
            var creator = new FlowRunProviderCreator();
            _provider = creator.GetFlowRunProvider();
            _serviceProvider = creator.ServiceProvider;
        }

        [Fact]
        public void ModelPropNestedTest()
        {
            var f = new TestForm1();
            var control = f.AddressLine1 as FieldBinding;
            Assert.Equal("$.Address.StreetLine1", control.Key);

            Assert.Equal("$.EffectiveDate", (f.EffectiveDateControl as FieldBinding).Key);

            var dropDown = f.ClientTypeControl as FieldBinding;
            Assert.Equal("$.ClientTypes", dropDown.ItemsBinding);
            Assert.Equal("$.Id", dropDown.IdBinding);
            Assert.Equal("$.Name", dropDown.NameBinding);
        }

        [Fact]
        public void ModelPropParserTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm1));
            Assert.True(details.Fields.Count > 0);

            var effectiveDateControl = details.Fields.FirstOrDefault(c => c.Name == "EffectiveDateControl");
            Assert.Equal("$.EffectiveDate", effectiveDateControl?.Binding?.Binding);
            Assert.Equal("DateEdit", effectiveDateControl?.ControlType);
        }

        [Fact]
        public void ButtonsParserTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm1));
            Assert.True(details.Fields.Count > 0);

            var cancelButton = details.Fields.FirstOrDefault(c => c.Name == "CancelButton");
            Assert.Equal(ModelBinding.CloseButtonBinding, cancelButton?.Binding?.Binding);
            Assert.Equal(FieldBindingType.ActionButton, cancelButton?.Binding?.BindingType);
            Assert.Equal("CancelButton", cancelButton?.DisplayProperties.Name);
            Assert.Equal("Cancel", cancelButton?.DisplayProperties.Caption);

            var submitButton = details.Fields.FirstOrDefault(c => c.Name == "SubmitButton");
            Assert.Equal(ModelBinding.SubmitButtonBinding, submitButton?.Binding?.Binding);
            Assert.Equal(FieldBindingType.ActionButton, submitButton?.Binding?.BindingType);
            Assert.Equal("SubmitButton", submitButton?.DisplayProperties.Name);
            Assert.Equal("Submit", submitButton?.DisplayProperties.Caption);
        }

        [Fact]
        public void DisplayAttributeParserTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm1));
            Assert.NotEmpty(details.Fields);

            var effectiveDateControl = details.Fields.FirstOrDefault(c => c.Name == "EffectiveDateControl");
            Assert.Equal("Effective Date", effectiveDateControl.DisplayProperties.Caption);
            Assert.True(effectiveDateControl.DisplayProperties.Visible);
            Assert.True(effectiveDateControl.DisplayProperties.Required);
            
            var lastNameControl = details.Fields.FirstOrDefault(c => c.Name == "LastNameControl");
            Assert.True(lastNameControl.DisplayProperties.Required);

            var clientTypeControl = details.Fields.FirstOrDefault(c => c.Name == "ClientTypeControl");
            Assert.False(clientTypeControl.DisplayProperties.Required);
            Assert.False(clientTypeControl.DisplayProperties.Visible);
       }

        [Fact]
        public void ParseFormWithoutControlsTest()
        {
            var parser = new FormDefinitionParser(_serviceProvider);
            var details = parser.Parse(typeof(TestForm2NoControls));
            Assert.NotEmpty(details.Fields);
        }
    }

    public class TestForm2NoControls : FlowTaskDefinitionBase<FormTestModel>
    {
        public override string Name => "TestForm";

        [Display("Effective Date", Required = true)]
        public object EffectiveDateControl => ModelProp(e => e.EffectiveDate);

        public object AddressControl => ModelProp(m => m.Address);

        public object AddressLine1 => ModelProp(m => m.Address.StreetLine1);

        [ComponentGroup(nameof(NameGroup))]
        public object FirstNameControl => ModelProp(m => m.Client.FirstName);

        [ComponentGroup(nameof(NameGroup))]
        [Display(Required = true)]
        public object LastNameControl => ModelProp(m => m.Client.LastName);

        [Display(Visible = false)]
        public object ClientTypeControl => SingleSelect(m => m.ClientTypeId, m => m.ClientTypes, i => i.Id, i => i.Name);

        public string NameGroup;
        public string AddressGroup;
    }

    public class FormTestModel : FlowModelBase
    {
        public string UserName { get; set; }
        public DateTime EffectiveDate { get; set; }

        public List<ClientType> ClientTypes { get; set; }
        public int ClientTypeId { get; set; }

        public ClientDetails Client { get; set; } = new ClientDetails();
        public AddressDetails Address { get; set; } = new AddressDetails();
    }

    public class ClientDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AddressDetails
    {
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
    }

    public class ClientType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TestForm1 : FlowTaskDefinitionBase<FormTestModel>
    {
        public override string Name => "TestForm";

        [FormComponent(typeof(DateEdit))]
        [Display("Effective Date", Required = true)]
        public object EffectiveDateControl => ModelProp(e => e.EffectiveDate);

        [FormComponent(typeof(AddressDetailsCustomComponent), Group = nameof(AddressGroup))]
        public object AddressControl => ModelProp(m => m.Address);
        
        public object AddressLine1 => ModelProp(m => m.Address.StreetLine1);

        [ComponentGroup(nameof(NameGroup))]
        [FormComponent(typeof(TextEdit))]
        public object FirstNameControl => ModelProp(m => m.Client.FirstName);

        [ComponentGroup(nameof(NameGroup))]
        [FormComponent(typeof(TextEdit))]
        [Display(Required = true)]
        public object LastNameControl => ModelProp(m => m.Client.LastName);

        [FormComponent(typeof(DropDown))]
        [Display(Visible = false)]
        public object ClientTypeControl => SingleSelect(m => m.ClientTypeId, m => m.ClientTypes, i => i.Id, i => i.Name);

        [Display("Cancel")]
        public object CancelButton => ActionButton(ActionType.Close);
        [Display("Submit")]
        public object SubmitButton => ActionButton(ActionType.Submit);

        public string NameGroup;
        public string AddressGroup;
    }

    // custom components
    public class AddressDetailsCustomComponent : IFormComponent
    {
        public string GetFullName()
        {
            return "AddressDetailsCustomComponent";
        }
    }

}
