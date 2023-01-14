using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Rendering.Model;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Infrastructure;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
	public class LeadBoardStateFlow : StateFlowBase<LeadBoardCardModel>
	{
        private readonly IClientCompanyRepository _clientCompanyRepository;
        private readonly ICompanyRepository _companyRepository;

		// Board Columns
		public state Lead;
		public state Contacted;
		public state MeetingScheduled = new state("Meeting Scheduled");
		public state ProposalDelivered = new state("Proposal Delivered");
		public state Won;

        public LeadBoardStateFlow(ICompanyRepository companyRepository, IClientCompanyRepository clientCompanyRepository)
        {
			_companyRepository = companyRepository;
            _clientCompanyRepository = clientCompanyRepository;
        }

        // Board Card Transitions
        public override void Define()
		{
			this
				.SetEditForm<FormLeadCardEdit>()
				// ToDo: add event that is executed each time when flow model is changed
				//.SetOnChange(BoardChangedAsync)
				.State(Lead)
					.TransitionForm<FormContactedCardEdit>(new UserActionTransitionTrigger(), Contacted, OnContactedAsync)
				.State(Contacted)
					.SetEditForm<FormContactedCardEdit>()
					// ToDo: add Events that executed when flow is in the paticular state (Contacted) and trigger condition is met
					// (for now events can be run when user opens the board)
					// in future we need a background service that checks events regularly
					//.Event(new ConditionTransitionTrigger(() => Model.FollowUpDate < DateTime.Now), () => Model.Status = Overdue)
					.Transition<UserActionTransitionTrigger>(Lead)
					.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
				.State(MeetingScheduled)
                    .Transition<UserActionTransitionTrigger>(Contacted)
                    .Transition<UserActionTransitionTrigger>(ProposalDelivered)
				.State(ProposalDelivered)
                    .Transition<UserActionTransitionTrigger>(MeetingScheduled)
					.TransitionForm<FormCardCommit>(new UserActionTransitionTrigger(), Won, SaveClientRecordAsync)
				.State(Won)
                    .Transition<UserActionTransitionTrigger>(Lead)
                    .Transition<UserActionTransitionTrigger>(Contacted)
                    .Transition<UserActionTransitionTrigger>(MeetingScheduled)
                    .Transition<UserActionTransitionTrigger>(ProposalDelivered)
					.End();
		}

		private async Task OnContactedAsync()
		{
		}

		private async Task SaveClientRecordAsync()
		{
		}
	}

	public class FormLeadCardEdit : FormEditBase<LeadBoardCardModel>
	{
		protected override void Define(FormEntityTypeBuilder<LeadBoardCardModel> f)
		{
			f.DisplayName = "Lead Card";
            f.Property(p => p.State).IsReadOnly();
			MainSection(f);
        }

		public static void MainSection(FormEntityTypeBuilder<LeadBoardCardModel> f)
		{
            //f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);
            f.Property(p => p.Title).IsRequired();
            f.Property(p => p.Description);
            f.Property(p => p.SalesPersonId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Sales person").IsRequired();
            f.Property(p => p.LeadSourceTypeId).Dropdown(p => p.AllLeadSources, m => m.Id, m => m.Name).Label("Lead source");
            f.Property(p => p.RelatedPersonId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Lead Contact");
            f.Property(p => p.RelatedCompanyId).DropdownSearch(p => p.AllCompanies, m => m.Id, m => m.Name).Label("Company");
            f.Property(p => p.Phone);
            f.Property(p => p.Email);
            f.Property(p => p.ContactDetails).Label("Other contact info");
            f.Property(p => p.Comments).Control(ControlType.TextArea);

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");
        }
	}

	public class FormContactedCardEdit : FormEditBase<LeadBoardCardModel>
	{
		protected override void Define(FormEntityTypeBuilder<LeadBoardCardModel> f)
		{
			f.DisplayName = "Lead Contacted Card";
			f.Property(p => p.State).IsReadOnly();
			f.Property(p => p.FollowUpDate).Label("Follow up date");
			f.Property(p => p.FollowUpDetails).Label("Follow up details");
			FormLeadCardEdit.MainSection(f);
        }
	}

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

    public class LoadIsNewClientCompanyRule : FlowRuleBase<LeadBoardCardModel>
    {
        public override string RuleCode => "BRD-1";

        public override void Execute(LeadBoardCardModel model)
        {
			model.ClientCompany.StartContractDate = DateTime.UtcNow;
            model.IsNewCompany = model.RelatedCompanyId == null;
        }
    }

    public class ClientCompanyExistsRule : FlowRuleAsyncBase<LeadBoardCardModel>
    {
		private readonly IClientCompanyRepository _clientCompanyRepository;
        public override string RuleCode => "BRD-2";

		public ClientCompanyExistsRule(IClientCompanyRepository clientCompanyRepository)
		{
			_clientCompanyRepository = clientCompanyRepository;
		}

        public override async Task Execute(LeadBoardCardModel model)
        {
			var cc = await _clientCompanyRepository.FindByCompanyIdAsync(model.RelatedCompanyId ?? 0);

			if (cc != null)
			{
				// disable change fields
				model.IsNewCompany = false;
				Result.Fields[SingleField(m => m.IsNewCompany)].Disabled = true;
				Result.Fields[SingleField(m => m.RelatedCompanyId)].Disabled = true;

				// load Client Company details
				cc.ReflectionCopyTo(model.ClientCompany);
            }
        }
    }

    public class NewClientCompanyRule : FlowRuleBase<LeadBoardCardModel>
    {
        public override string RuleCode => "BRD-3";

        public override void Execute(LeadBoardCardModel model)
        {
            Result.Fields[SingleField(m => m.RelatedCompanyId)].Visible = !model.IsNewCompany;
            Result.Fields[SingleField(m => m.RelatedCompanyId)].Required = !model.IsNewCompany;

            Result.Fields[SingleField(m => m.Company.Name)].Visible = model.IsNewCompany;
            Result.Fields[SingleField(m => m.Company.RegistrationNumber)].Visible = model.IsNewCompany;
            Result.Fields[SingleField(m => m.Company.EstablishedDate)].Visible = model.IsNewCompany;
        }
    }
}
