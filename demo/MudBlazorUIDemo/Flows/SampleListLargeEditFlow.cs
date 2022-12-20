using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;

namespace MudBlazorUIDemo.Flows
{
    public class SampleListLargeEditFlow : FluentFlowBase<CustomerModel>
    {
        private static bool _blink = true;

        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(SampleListLargeEditForm))
                //.NextForm(typeof(TestCustAddrCountFormList))
                .End(SaveData);
        }

        public async Task LoadData()
        {
            if (Params.ItemId == "101")
            {
                Model.FirstName = "Ben";
                Model.LastName = "Jones";
                Model.AddrCount = 4;
            }

            _blink = !_blink;

            if (_blink)
            {
                // throw new Exception("Dialog Flow ERROR 5");
            }
        }

        public async Task SaveData()
        {
        }
    }

    public class SampleListLargeEditForm : FormEditBase<CustomerModel>
    {
        protected override void Define(FormEntityTypeBuilder<CustomerModel> f)
        {
            f.DisplayName = "Client Edit Form";

            f.Property(p => p.FirstName).IsRequired();
            f.Property(p => p.LastName).IsRequired();
            f.Property(p => p.DOB).IsRequired();
            f.Property(p => p.ModifiedDate).Format("dd/MM/yyyy");
            f.Property(p => p.AddrCount).Rule(typeof(SampleListLargeEditRule11), FormRuleTriggers.Loaded);

            // Additional Buttons invisible in Dialog Mode
            f.Button(ButtonActionTypes.Close, "OK");

            f.Rule(typeof(SampleEditFormExampleRule), FormRuleTriggers.Loaded);
            //f.Rule(typeof(SampleListDisableButtonsRule), FormRuleTriggers.Loaded);
        }
    }

    public class SampleListLargeEditRule11 : FlowRuleAsyncBase<CustomerModel>
    {
        public override string RuleCode => this.GetType().Name;

        public override async Task Execute(CustomerModel model)
        {
            if (model.AddrCount == 0)
            {
                model.AddrCount = 1;
            }
        }
    }

    public class SampleEditFormExampleRule : FlowRuleAsyncBase<CustomerModel>
    {
        public override string RuleCode => "SLLR-004";

        public override async Task Execute(CustomerModel model)
        {
        }
    }
}
