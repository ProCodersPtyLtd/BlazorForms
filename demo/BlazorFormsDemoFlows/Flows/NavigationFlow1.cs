using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Platform.Crm.Business.Artel;
using BlazorForms.Platform.Crm.Domain.Models.Artel;
using BlazorFormsDemoModels.Models;

namespace BlazorFormsDemoFlows.Flows
{
    public class NavigationFlow1 : FluentFlowBase<NavigationModel1>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(() => LoadData())
                .NextForm(typeof(NavigationForm1))
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                    .Next(() => Default2())
                    .NextForm(typeof(NavigationForm2))
                    .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                        .NextForm(typeof(NavigationForm3))
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

    public class NavigationForm1 : FormEditBase<NavigationModel1>
    {
        protected override void Define(FormEntityTypeBuilder<NavigationModel1> builder)
        {
        }
    }

    public class NavigationForm2 : FormEditBase<NavigationModel1>
    {
        protected override void Define(FormEntityTypeBuilder<NavigationModel1> builder)
        {
        }
    }

    public class NavigationForm3 : FormEditBase<NavigationModel1>
    {
        protected override void Define(FormEntityTypeBuilder<NavigationModel1> builder)
        {
        }
    }
}
