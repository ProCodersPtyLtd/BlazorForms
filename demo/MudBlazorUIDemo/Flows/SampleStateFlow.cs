using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
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
			this.State(Leads)
				.Transition(new UserActionTransitionTrigger(), Contacted, OnAssigning)
			.State(Contacted)
				.Transition(new UserActionTransitionTrigger(), Leads)
				.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
			.State(MeetingScheduled)
				.Transition(new UserActionTransitionTrigger(), Contacted)
				.Transition(new UserActionTransitionTrigger(), ProposalDelivered)
			.State(ProposalDelivered)
				.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
				.Transition(new UserActionTransitionTrigger(), Won)
			.State(Won)
				.Transition(new UserActionTransitionTrigger(), Leads)
				.Transition(new UserActionTransitionTrigger(), Contacted)
				.Transition(new UserActionTransitionTrigger(), MeetingScheduled)
				.Transition(new UserActionTransitionTrigger(), ProposalDelivered)
				.End();
		}

		private void OnAssigning()
		{
		}
	}

	public class SampleStateModel : IFlowModel
	{ }
}
