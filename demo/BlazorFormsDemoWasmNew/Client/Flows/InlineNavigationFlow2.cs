using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorFormsDemoModels.Models;

namespace BlazorFormsDemoWasmNew.Client.Flows
{
    public class InlineNavigationFlow2 : FluentFlowBase<NavigationModel1>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(() => LoadData())
                .NextForm("InlineForm1")
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                    .Next(() => Default2())
                    .NextForm("InlineForm2")
                    .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                        .NextForm("InlineForm3")
                    .EndIf()
                .EndIf()
                .End();
        }

        private async Task LoadData() 
        { 
        }

        private async Task Default2() 
        {
            Model.Continue = true;
            Model.WelcomeText = $"Welcome {Model.UserName}";
        }
    }

}
