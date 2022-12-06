using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Admin.BusinessObjects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Admin.BusinessObjects.RegisteredFlows
{
    public class RegisteredListItemViewFlow : FluentFlowBase<FlowDataDetails>
    {
        private static bool _blink = true;

        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(RegisteredListItemViewFlowForm))
                .End(SaveData);
        }

        public async Task LoadData()
        {
        }

        public async Task SaveData()
        {
        }
    }

    public class RegisteredListItemViewFlowForm : FormEditBase<FlowDataDetails>
    {
        protected override void Define(FormEntityTypeBuilder<FlowDataDetails> f)
        {
            f.DisplayName = "Client Address Count";

            //f.Property(p => p.FirstName).IsRequired();
            //f.Property(p => p.LastName).IsRequired();
            //f.Property(p => p.AddrCount).Rule(typeof(SampleListEditRule11), FormRuleTriggers.Loaded);

            // Additional Buttons invisible in Dialog Mode
            f.Button(ButtonActionTypes.Close, "OK");

            //f.Rule(typeof(SampleEditFormExampleRule), FormRuleTriggers.Loaded);
            //f.Rule(typeof(SampleListDisableButtonsRule), FormRuleTriggers.Loaded);
        }
    }
}
