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
				.Begin(OnStartAsync)
				.SetEditForm<FormCardEdit>()
				.State(Leads)
					.Transition(new UserActionTransitionTrigger(), Contacted, OnAssigning)
				.State(Contacted)
					.Begin(OnStartContactedAsync)
					.Transition(new UserActionTransitionTrigger(), Leads)
					.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
				.State(MeetingScheduled)
					.Transition(new UserActionTransitionTrigger(), Contacted)
					.Transition(new UserActionTransitionTrigger(), ProposalDelivered)
				.State(ProposalDelivered)
					.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
					.Transition(new UserActionTransitionTrigger(), Won, OnCloseAsync)
					//.TransitionForm<FormCardCommit>(new UserActionTransitionTrigger(), Won, OnCloseAsync)
				.State(Won)
					.Transition(new UserActionTransitionTrigger(), Leads)
					.Transition(new UserActionTransitionTrigger(), Contacted)
					.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
					.Transition(new UserActionTransitionTrigger(), ProposalDelivered)
					.End();
		}

		private async Task OnStartAsync()
		{
			Model.CloseMessage = "Congrats with another win! Click [Ok] to close the card.";
		}

		private async Task OnStartContactedAsync()
		{
		}

		private void OnAssigning()
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
			f.Property(p => p.CloseMessage).Control(ControlType.Paragraph);

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
