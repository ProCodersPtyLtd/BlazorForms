using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Rendering.Model;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MudBlazorUIDemo.Flows
{
	public class SampleStateFlow : StateFlowBase<SampleStateModel>
	{
		// Board Columns
		public state Leads;
		public state Contacted;
		public state MeetingScheduled = new state("Meeting Scheduled");
		public state ProposalDelivered = new state("Proposal Delivered");
		public state Won;

		// Board Card Transitions
		public override void Define()
		{
			this
				//.BeginState(OnStartAsync)
				.SetEditForm<FormCardEdit>()
				.State(Leads)
					// generic parameter example
					.Transition<UserActionTransitionTrigger>(Contacted, OnContactedAsync)
				.State(Contacted)
                    .Transition<UserActionTransitionTrigger>(Leads)
					// supplying object example
					.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
				.State(MeetingScheduled)
                    .Transition<UserActionTransitionTrigger>(Contacted)
                    .Transition<UserActionTransitionTrigger>(ProposalDelivered)
				.State(ProposalDelivered)
					//.BeginState(OnProposalDeliveredAsync)
                    .Transition<UserActionTransitionTrigger>(MeetingScheduled)
					.TransitionForm<FormCardCommit>(new UserActionTransitionTrigger(), Won, OnCloseAsync)
				.State(Won)
                    .Transition<UserActionTransitionTrigger>(Leads)
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
			Model.CloseMessage = "Congrats with another win! Click [Ok] to close the card.";
		}

		private async Task OnContactedAsync()
		{
		}

		private async Task OnCloseAsync()
		{ 
		}
	}

	public class FormCardEdit : FormEditBase<SampleStateModel>
	{
		protected override void Define(FormEntityTypeBuilder<SampleStateModel> f)
		{
			f.Property(p => p.State).IsReadOnly();
			f.Property(p => p.Title).IsRequired();
			f.Property(p => p.Description).IsRequired();

			f.Button(ButtonActionTypes.Cancel, "Cancel");
			f.Button(ButtonActionTypes.Submit, "Save");
		}
	}

	public class FormCardCommit : FormEditBase<SampleStateModel>
	{
		protected override void Define(FormEntityTypeBuilder<SampleStateModel> f)
		{
			f.DisplayName = "Congrats with another win! Click [OK] to close the card.";
			f.Property(p => p.Title).IsReadOnly().Label("Card");
			//f.Property(p => p.CloseMessage).Control(ControlType.Subtitle);

			f.Button(ButtonActionTypes.Cancel, "Cancel");
			f.Button(ButtonActionTypes.Submit, "Ok");
		}
	}

	public class SampleStateModel : IFlowBoardCard
	{
		public string State { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }

		public string CloseMessage { get; set; }
	}
}
