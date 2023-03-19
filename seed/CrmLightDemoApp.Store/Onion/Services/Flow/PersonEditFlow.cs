using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Storage;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow
{
    public class PersonEditFlow : FluentFlowBase<PersonModel>
    {
        private readonly IHighStore _store;

        public PersonEditFlow(IHighStore store)
        {
            _store = store;
        }

        public override void Define()
        {
            this
                .Begin()
                .If(() => _flowContext.Params.ItemKeyAboveZero)
                   .Next(LoadData)
                   .NextForm(typeof(FormPersonView))
                .EndIf()
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.DeleteButtonBinding)
                    .Next(DeleteData)
                .Else()
                    .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding || !_flowContext.Params.ItemKeyAboveZero)
                        .NextForm(typeof(FormPersonEdit))
                        .Next(SaveData)
                    .EndIf()
                .EndIf()
                .End();
        }

        public async Task LoadData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                Model = await _store.GetByIdAsync<PersonModel>(_flowContext.Params.ItemKey);
            }
        }

        public async Task DeleteData()
        {
            await _store.SoftDeleteAsync(Model);
        }

        public async Task SaveData()
        {
            await _store.UpsertAsync(Model);
        }
    }

    public class FormPersonView : FormEditBase<PersonModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonModel> f)
        {
            f.DisplayName = "Person View";

            f.Property(p => p.FirstName).Label("First name").IsReadOnly();
            f.Property(p => p.LastName).Label("Last name").IsReadOnly();
            f.Property(p => p.BirthDate).Label("Date of birth").IsReadOnly();
            //f.Property(p => p.Fomo.RoleName).Label("Fomo name").IsReadOnly();
            f.Property(p => p.Phone).IsReadOnly();
            f.Property(p => p.Email).IsReadOnly();

            f.Button(ButtonActionTypes.Submit, "Edit");

            f.Button(ButtonActionTypes.Delete, "Delete")
                .Confirm(ConfirmType.Delete, "Delete this Person?", ConfirmButtons.YesNo);

            f.Button(ButtonActionTypes.Close, "Close");
        }
    }

    public class FormPersonEdit : FormEditBase<PersonModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonModel> f)
        {
            f.DisplayName = "Person Edit";
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);

            f.Property(p => p.FirstName).Label("First name").IsRequired();
            f.Property(p => p.LastName).Label("Last name").IsRequired();
            f.Property(p => p.BirthDate).Label("Date of birth");
            f.Property(p => p.Phone);
            f.Property(p => p.Email);

            f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");
        }
    }
}
