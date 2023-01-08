using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Rendering.Model;
using CrmLightDemoApp.Onion.Services.Model;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace CrmLightDemoApp.Onion.Services.Flow
{
	public class BoardStateFlow : StateFlowBase<BoardCardModel>
	{
		// Board Columns
		public state Lead;
		public state Contacted;
		public state MeetingScheduled = new state("Meeting Scheduled");
		public state ProposalDelivered = new state("Proposal Delivered");
		public state Won;

		// Board Card Transitions
		public override void Define()
		{
			this
				.Begin(OnStartAsync)
				.SetEditForm<FormLeadCardEdit>()
				.State(Lead)
					// generic trigger generic parameter example
					.Transition<UserActionTransitionTrigger>(Contacted, OnContactedAsync)
				.State(Contacted)
                    .Transition<UserActionTransitionTrigger>(Lead)
					// supplying trigger object example
					.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
				.State(MeetingScheduled)
                    .Transition<UserActionTransitionTrigger>(Contacted)
                    .Transition<UserActionTransitionTrigger>(ProposalDelivered)
				.State(ProposalDelivered)
					.Begin(OnProposalDeliveredAsync)
                    .Transition<UserActionTransitionTrigger>(MeetingScheduled)
					.TransitionForm<FormCardCommit>(new UserActionTransitionTrigger(), Won, OnCloseAsync)
				.State(Won)
                    .Transition<UserActionTransitionTrigger>(Lead)
                    .Transition<UserActionTransitionTrigger>(Contacted)
                    .Transition<UserActionTransitionTrigger>(MeetingScheduled)
                    .Transition<UserActionTransitionTrigger>(ProposalDelivered)
					.End();
		}

		private async Task OnStartAsync()
		{
		}

		private async Task OnProposalDeliveredAsync()
		{
		}

		private async Task OnContactedAsync()
		{
		}

		private async Task OnCloseAsync()
		{ 
		}
	}

	public class FormLeadCardEdit : FormEditBase<BoardCardModel>
	{
		protected override void Define(FormEntityTypeBuilder<BoardCardModel> f)
		{
			f.Property(p => p.State).IsReadOnly();
			f.Property(p => p.Title).IsRequired();
			f.Property(p => p.Description);
			f.Property(p => p.SalesPersonId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Sales person").IsRequired();
			f.Property(p => p.LeadSourceTypeId).Dropdown(p => p.AllLeadSources, m => m.Id, m => m.Name).Label("Lead source");

			f.Button(ButtonActionTypes.Cancel, "Cancel");
			f.Button(ButtonActionTypes.Submit, "Save");
		}
	}

	public class FormCardCommit : FormEditBase<BoardCardModel>
	{
		protected override void Define(FormEntityTypeBuilder<BoardCardModel> f)
		{
			f.DisplayName = "Congrats with another win! Click OK to close the card.";
			f.Property(p => p.Title).IsReadOnly().Label("Card");

			f.Button(ButtonActionTypes.Cancel, "Cancel");
			f.Button(ButtonActionTypes.Submit, "Ok");
		}
	}
}
