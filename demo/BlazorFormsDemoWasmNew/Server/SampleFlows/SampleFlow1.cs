using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.ProcessFlow;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms
{
    [Form("Project Settings")]
    public class SampleForm1 : FlowTaskDefinitionBase<SampleModel1>
    {
        [FormComponent(typeof(TextEdit))]
        [Display("Name")]
        [FlowRule(typeof(NameChangedRule), FormRuleTriggers.Changed)]
        public object NameControl => ModelProp(m => m.Name);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Initial Share Price")]
        public object SharePriceControl => ModelProp(m => m.Amount);

        [Display("Cancel")]
        public object CancelButton => ActionButton(ActionType.Close);
        [Display("Submit")]
        public object SubmitButton => ActionButton(ActionType.Submit);
    }

    public class NameChangedRule : FlowRuleAsyncBase<SampleModel1>
    {
        public override string RuleCode => typeof(NameChangedRule).FullName;

        public override async Task Execute(SampleModel1 model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                Result.ValidationResult = RuleValidationResult.Error;
                Result.ValidationMessage = "Please enter a valuable Name";
            }
        }
    }

    public class SampleFlow1 : FluentFlowBase<SampleModel1>
    {
        public override void Define()
        {
            this
                .Begin(FlowStart)
                .Next(LoadData)
                .NextForm(typeof(SampleForm1))
                .Next(SaveData)
                .End(FlowEnd);
        }

        public async Task FlowStart()
        {
        }

        public async Task LoadData()
        {
            Model.Name = "DeleteMe";
            Model.Amount = new Money(100m, "Tanga");
        }

        public async Task SaveData()
        {
        }

        public async Task FlowEnd()
        {
        }
    }
}
