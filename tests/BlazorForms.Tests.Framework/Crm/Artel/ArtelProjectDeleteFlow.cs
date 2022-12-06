using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Crm.Artel;

namespace BlazorForms.Platform.Crm.Business.Artel
{
    [Flow(nameof(ArtelProjectDeleteFlow))]
    public class ArtelProjectDeleteFlow : FluentFlowBase<ArtelProjectSettingsModel>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(LoadData)
                .NextForm(typeof(FormArtelProjectSettings))
                .Next(SaveAsync)
                .End();
        }

        private async Task LoadData()
        {

        }

        private async Task SaveAsync()
        {

        }
    }
}
