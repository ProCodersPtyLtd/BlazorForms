using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class PersonEditFlow : FluentFlowBase<PersonModel>
    {
        private readonly IPersonRepository _personRepository;

        public PersonEditFlow(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public override void Define()
        {
            this
                .If(() => _flowContext.Params.ItemKeyAboveZero)
                   .Begin(LoadData)
                   .NextForm(typeof(FormPersonView))
                .EndIf()
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding || !_flowContext.Params.ItemKeyAboveZero)
                    .NextForm(typeof(FormPersonEdit))
                    .Next(SaveData)
                .EndIf()
                .End();
        }

        public async Task LoadData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                var item = await _personRepository.GetByIdAsync(_flowContext.Params.ItemKey);
                item.CopyTo(Model);
            }
        }

        public async Task SaveData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                await _personRepository.UpdateAsync(Model);
            }
            else
            {
                await _personRepository.CreateAsync(Model);
            }
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
            f.Property(p => p.Phone).IsReadOnly();
            f.Property(p => p.Email).IsReadOnly();

            f.Button(ButtonActionTypes.Submit, "Edit");
            f.Button(ButtonActionTypes.Close, "Close");

        }
    }

    public class FormPersonEdit : FormEditBase<PersonModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonModel> f)
        {
            f.DisplayName = "Person Edit";

            f.Property(p => p.FirstName).Label("First name").IsRequired();
            f.Property(p => p.LastName).Label("Last name").IsRequired();
            f.Property(p => p.BirthDate).Label("Date of birth").IsRequired();
            f.Property(p => p.Phone);
            f.Property(p => p.Email);

            f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");

        }
    }
}
