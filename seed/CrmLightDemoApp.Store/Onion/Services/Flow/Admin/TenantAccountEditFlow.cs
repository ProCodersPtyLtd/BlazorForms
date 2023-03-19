using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Infrastructure;
using CrmLightDemoApp.Store.Onion.Services.Abstractions;
using CrmLightDemoApp.Store.Onion.Services.Flow.LeadBoard;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow.Admin
{
    public class TenantAccountEditFlow : FluentFlowBase<TenantAccountModel>
    {
        private readonly ITenantAccountRepository _tenantRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly INotificationService _notificationService;

        public TenantAccountEditFlow(ITenantAccountRepository tenantRepository, ICompanyRepository companyRepository,
			INotificationService notificationService)
        {
			_tenantRepository = tenantRepository;
            _companyRepository = companyRepository;
            _notificationService = notificationService;
		}

        public override void Define()
        {
            this
                .Begin()
                .Next(LoadDataAsync)
                .NextForm(typeof(FormTenantAccountEdit))
                .Next(SaveDataAsync)
                .End();
        }

        public async Task LoadDataAsync()
        {
            Model = TenantAccountModel.FromDetails(await _tenantRepository.GetTenantAccountDetailsAsync());
		}
 
        public async Task SaveDataAsync()
        {
			await _tenantRepository.UpdateAsync(Model);
            await _companyRepository.UpdateAsync(Model.Company);

            await _notificationService.PostMessageAsync(new MessageEventArgs { Type = MessageEventType.TenantAccount });
		}
    }

    public class FormTenantAccountEdit : FormEditBase<TenantAccountModel>
    {
        protected override void Define(FormEntityTypeBuilder<TenantAccountModel> f)
        {

            f.DisplayName = "Account Edit";
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);

			f.Property(p => p.Company.Name).Label("RoleName").IsPrimaryKey().IsRequired();
			f.Property(p => p.Company.RegistrationNumber).Label("Reg. No.");
			f.Property(p => p.Company.EstablishedDate).Label("Established date");

			f.Property(p => p.Bio).Label("BIO").Control(ControlType.TextArea);

			f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");
        }
    }
}
