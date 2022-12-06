using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class PersonEditFlow : FluentFlowBase<PersonModel>
    {
        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(FormPersonView))
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding)
                    .NextForm(typeof(FormPersonEdit))
                    .Next(SaveData)
                .EndIf()
                .End();
        }

        public async Task LoadData()
        {
        }

        public async Task SaveData()
        {
        }
    }

    public class FormPersonView : FormEditBase<PersonModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonModel> f)
        {
            f.DisplayName = "Person View";

            f.Property(p => p.FirstName).IsReadOnly();
            f.Property(p => p.LastName).IsReadOnly();
            f.Property(p => p.BirthDate).IsReadOnly();

            f.Button(ButtonActionTypes.Submit, "Edit");
            f.Button(ButtonActionTypes.Close, "Close");

        }
    }

    public class FormPersonEdit : FormEditBase<PersonModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonModel> f)
        {
            f.DisplayName = "Person Edit";

            f.Property(p => p.FirstName).IsRequired();
            f.Property(p => p.LastName).IsRequired();
            f.Property(p => p.BirthDate);

            f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");

        }
    }
}
