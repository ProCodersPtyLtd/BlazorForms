using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Shared;

namespace BlazorForms
{
    public class SampleSilentFluentForm : FormEditBase<SampleModel2>
    {
        protected override void Define(FormEntityTypeBuilder<SampleModel2> f)
        {
            f.DisplayName = "Sample Silent Fluent Form";

            f.Property(p => p.Name).Rule(typeof(SampleSilentForm1NameChangedRule2), FormRuleTriggers.Changed);
            f.Property(p => p.Amount).Control(typeof(MoneyEdit));

            f.Button("CustAddrCountList", ButtonActionTypes.Close, "Cancel");
            f.Button("CustAddrCountList", ButtonActionTypes.Submit);
        }
    }
 
    public class SampleSilentForm1NameChangedRule2 : FlowRuleAsyncBase<SampleModel2>
    {
        public override string RuleCode => this.GetType().FullName;

        public override async Task Execute(SampleModel2 model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                Result.ValidationResult = RuleValidationResult.Error;
                Result.ValidationMessage = "Please enter a valuable Name";
            }
        }
    }

    public class SampleSilentFlow2 : FluentFlowBase<SampleModel2>
    {
        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(SampleSilentFluentForm))
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
