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
    public class InlineNavigationFlow1 : FluentFlowBase<NavigationModel1>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(() => LoadData())
                .NextForm(typeof(InlineForm1))
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                    .Next(() => Default2())
                    .NextForm(typeof(InlineForm2))
                    .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                        .NextForm(typeof(InlineForm3))
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

    public class InlineForm1 : FormEditBase<NavigationModel1>
    {
        protected override void Define(FormEntityTypeBuilder<NavigationModel1> builder)
        {
        }
    }

    public class InlineForm2 : FormEditBase<NavigationModel1>
    {
        protected override void Define(FormEntityTypeBuilder<NavigationModel1> builder)
        {
        }
    }

    public class InlineForm3 : FormEditBase<NavigationModel1>
    {
        protected override void Define(FormEntityTypeBuilder<NavigationModel1> builder)
        {
        }
    }
}
