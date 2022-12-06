using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;

namespace BlazorForms
{
    public class SampleListEditFlow : FluentFlowBase<CustAddrCount>
    {
        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(SampleListEditForm))
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
        }

        public async Task SaveData()
        {
        }
    }

    public class SampleListEditForm : FormEditBase<CustAddrCount>
    {
        protected override void Define(FormEntityTypeBuilder<CustAddrCount> f)
        {
            f.DisplayName = "Client Address Count";

            f.Property(p => p.FirstName).IsRequired();
            f.Property(p => p.LastName).IsRequired();
            f.Property(p => p.AddrCount).Rule(typeof(SampleListEditRule1), FormRuleTriggers.Loaded);
        }
    }

    public class SampleListEditRule1 : FlowRuleAsyncBase<CustAddrCount>
    {
        public override string RuleCode => this.GetType().Name;

        public override async Task Execute(CustAddrCount model)
        {
            if (model.AddrCount == 0)
            {
                model.AddrCount = 1;
            }
        }
    }
}
