using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class CompanyEditFlow : FluentFlowBase<CompanyModel>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IPersonCompanyRepository _personCompanyRepository;

        public CompanyEditFlow(ICompanyRepository companyRepository, IPersonCompanyRepository personCompanyRepository)
        {
            _companyRepository = companyRepository;
            _personCompanyRepository = personCompanyRepository;
        }

        public override void Define()
        {
            this
                .If(() => _flowContext.Params.ItemKeyAboveZero)
                   .Begin(LoadData)
                   .NextForm(typeof(FormCompanyView))
                .EndIf()
                .If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.SubmitButtonBinding || !_flowContext.Params.ItemKeyAboveZero)
                    .NextForm(typeof(FormCompanyEdit))
                    .Next(SaveData)
                .EndIf()
                .End();
        }

        public async Task LoadData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                var item = await _companyRepository.GetByIdAsync(_flowContext.Params.ItemKey);
                // item and Model have different types - we use reflection to copy similar properties
                item.ReflectionCopyTo(Model);
                Model.PersonCompanyLinks = await _personCompanyRepository.GetByCompanyIdAsync(Model.Id);
            }
        }

        public async Task SaveData()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                await _companyRepository.UpdateAsync(Model);
            }
            else
            {
                await _companyRepository.CreateAsync(Model);
            }
        }
    }

    public class FormCompanyView : FormEditBase<CompanyModel>
    {
        protected override void Define(FormEntityTypeBuilder<CompanyModel> f)
        {
            f.DisplayName = "Company View";
            f.Property(p => p.Name).Label("Name").IsReadOnly();
            f.Property(p => p.RegistrationNumber).Label("Reg. No.").IsReadOnly();
            f.Property(p => p.EstablishedDate).Label("Established date").IsReadOnly();

            f.Table(p => p.PersonCompanyLinks, e => 
            {
                e.DisplayName = "Associations";
                e.Property(p => p.LinkTypeName).Label("Type");
                e.Property(p => p.PersonFullName).Label("Person");
            });

            f.Button(ButtonActionTypes.Close, "Close");
            f.Button(ButtonActionTypes.Submit, "Edit");

        }
    }

    public class FormCompanyEdit : FormEditBase<CompanyModel>
    {
        protected override void Define(FormEntityTypeBuilder<CompanyModel> f)
        {
            f.DisplayName = "Company Edit";

            f.Property(p => p.Name).Label("Name").IsRequired();
            f.Property(p => p.RegistrationNumber).Label("Reg. No.").IsRequired();
            f.Property(p => p.EstablishedDate).Label("Established date").IsRequired();

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");

        }
    }
}
