using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow.LeadBoard
{
    public class FormCardCommit : FormEditBase<LeadBoardCardModel>
    {
        protected override void Define(FormEntityTypeBuilder<LeadBoardCardModel> f)
        {
            f.DisplayName = "Congrats with another win! Click 'Save' to create client record.";
            //f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);
            f.Rule(typeof(LoadIsNewClientCompanyRule), FormRuleTriggers.Loaded);
            f.Rule(typeof(ClientCompanyExistsRule), FormRuleTriggers.Loaded);
            f.Property(p => p.Title).IsReadOnly();

            f.Property(p => p.ClientCompany.StartContractDate).Label("Start contract date");
            f.Property(p => p.ClientCompany.ClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Client manager");
            f.Property(p => p.ClientCompany.AlternativeClientManagerId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Alternative client manager");

            f.Property(p => p.IsNewCompany).Label("Create new company")
                .Rule(typeof(NewClientCompanyRule))
                .Rule(typeof(NewClientCompanyRule), FormRuleTriggers.Loaded);

            f.Property(p => p.RelatedCompanyId).DropdownSearch(p => p.AllCompanies, m => m.Id, m => m.Name).Label("Existing company");
            f.Property(p => p.Company.Name).Label("Comany name").IsRequired();
            f.Property(p => p.Company.RegistrationNumber).Label("Reg. No.").IsRequired();
            f.Property(p => p.Company.EstablishedDate).Label("Established date");

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");
        }
    }
}
