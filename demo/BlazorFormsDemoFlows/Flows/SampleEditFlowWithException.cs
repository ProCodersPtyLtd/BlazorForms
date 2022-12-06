using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorFormsDemoModels.Models;

namespace BlazorFormsDemoFlows.Flows
{
    public class SampleEditFlowWithException : FluentFlowBase<CustAddrCount>
    {
        private static bool _blink = true;

        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(SampleEditFlowWithExceptionForm))
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
                throw new Exception("Flow error 1");
            }
        }

        public async Task SaveData()
        {
        }
    }

    public class SampleEditFlowWithExceptionForm : FormEditBase<CustAddrCount>
    {
        protected override void Define(FormEntityTypeBuilder<CustAddrCount> f)
        {
            f.DisplayName = "Client Address Count";

            f.Property(p => p.FirstName).IsRequired();
            f.Property(p => p.LastName).IsRequired();
            f.Property(p => p.AddrCount).Rule(typeof(SampleEditFlowWithExceptionRule1), FormRuleTriggers.Loaded);

            // Additional Buttons invisible in Dialog Mode
            f.Button(ButtonActionTypes.Close, "OK");

            f.Rule(typeof(SampleEditFlowWithExceptionRule2), FormRuleTriggers.Loaded);
        }
    }

    public class SampleEditFlowWithExceptionRule1 : FlowRuleAsyncBase<CustAddrCount>
    {
        public override string RuleCode => "SLR-006";

        public override async Task Execute(CustAddrCount model)
        {
            throw new Exception("Loaded Rule error 2");

            if (model.AddrCount == 0)
            {
                model.AddrCount = 1;
            }
        }
    }

    public class SampleEditFlowWithExceptionRule2 : FlowRuleAsyncBase<CustAddrCount>
    {
        public override string RuleCode => "SLR-007";

        public override async Task Execute(CustAddrCount model)
        {
        }
    }
}
