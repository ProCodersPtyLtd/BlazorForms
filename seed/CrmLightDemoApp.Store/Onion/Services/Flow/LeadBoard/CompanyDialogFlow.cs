using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Infrastructure;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow.LeadBoard
{
    public class CompanyDialogFlow : DialogFlowBase<CompanyModel, FormCompanyDialogEdit>
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyDialogFlow(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public override async Task LoadDataAsync()
        {
            if (GetId() > 0)
            {
                var record = await _companyRepository.GetByIdAsync(GetId());
                record.ReflectionCopyTo(Model);
            }
            else
            {
                Model.Name = Params["RoleName"];
            }
        }

        public override async Task SaveDataAsync()
        {
            if (GetId() > 0)
            {
                await _companyRepository.UpdateAsync(Model);
            }
            else
            {
                Model.Id = await _companyRepository.CreateAsync(Model);
            }
        }
    }

    public class FormCompanyDialogEdit : FormEditBase<CompanyModel>
    {
        protected override void Define(FormEntityTypeBuilder<CompanyModel> f)
        {
            f.DisplayName = "Company Edit";
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);
            f.Property(p => p.Name).Label("RoleName").IsRequired();
            f.Property(p => p.RegistrationNumber).Label("Reg. No.");
            f.Property(p => p.EstablishedDate).Label("Established date");
            f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");
        }
    }
}
