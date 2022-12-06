using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms;

namespace BlazorForms
{
    [Form("Sample Silent Form")]
    public class SampleSilentForm1 : FlowTaskDefinitionBase<SampleModel2>
    {
        [FormComponent(typeof(TextEdit))]
        [Display("Name")]
        [FlowRule(typeof(SampleSilentForm1NameChangedRule), FormRuleTriggers.Changed)]
        public object NameControl => ModelProp(m => m.Name);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Initial Share Price")]
        public object SharePriceControl => ModelProp(m => m.Amount);

        [Display("Cancel")]
        public object CancelButton => ActionButton(ActionType.Close);
        [Display("Submit")]
        public object SubmitButton => ActionButton(ActionType.Submit);
    }

    public class SampleSilentForm1NameChangedRule : FlowRuleAsyncBase<SampleModel2>
    {
        public override string RuleCode => typeof(SampleSilentForm1NameChangedRule).FullName;

        public override async Task Execute(SampleModel2 model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                Result.ValidationResult = RuleValidationResult.Error;
                Result.ValidationMessage = "Please enter a valuable Name";
            }
        }
    }

    public class SampleSilentFlow : FluentFlowBase<SampleModel2>
    {
        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(SampleSilentForm1))
                .End(SaveData);
        }

        //public async Task FlowStart()
        //{
        //}

        public async Task LoadData()
        {
            if (FirstPass)
            {
                Model.Name = "DeleteMe";
                Model.Amount = new Money(100m, "Tanga");
            }
        }

        public async Task SaveData()
        {
        }

        //public async Task FlowEnd()
        //{
        //}
    }
}
