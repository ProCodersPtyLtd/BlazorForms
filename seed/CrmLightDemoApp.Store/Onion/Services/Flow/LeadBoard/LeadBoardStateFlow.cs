using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Rendering.Model;
using BlazorForms.Shared;
using CrmLightDemoApp.Store.Onion.Domain;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Infrastructure;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow.LeadBoard
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
					.TransitionForm<FormContactedCardEdit>(new UserActionTransitionTrigger(), Contacted)
				.State(Contacted)
					//.SetEditForm<FormContactedCardEdit>()
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
					.TransitionForm<FormCardCommit>(new UserActionTransitionTrigger(), Won)
				.State(Won)
                    //.Transition<UserActionTransitionTrigger>(Lead)
                    //.Transition<UserActionTransitionTrigger>(Contacted)
                    //.Transition<UserActionTransitionTrigger>(MeetingScheduled)
                    .Transition<UserActionTransitionTrigger>(ProposalDelivered)
					.End();
		}
	}
}
