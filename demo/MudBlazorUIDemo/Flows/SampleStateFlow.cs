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
				.Transition(new ButtonTransitionTrigger("Contacted"), Contacted, OnAssigning)
			.State(Contacted)
				.Transition(new ButtonTransitionTrigger("Leads"), Leads)
				.Transition(new ButtonTransitionTrigger("Meeting Scheduled"), MeetingScheduled)
			.State(MeetingScheduled)
				.Transition(new ButtonTransitionTrigger("Contacted"), Contacted)
				.Transition(new ButtonTransitionTrigger("Proposal Delivered"), ProposalDelivered)
			.State(ProposalDelivered)
				.Transition(new ButtonTransitionTrigger("Meeting Scheduled"), MeetingScheduled)
				.Transition(new ButtonTransitionTrigger("Won"), Won)
			.State(Won)
				.Transition(new ButtonTransitionTrigger("Leads"), Leads)
				.Transition(new ButtonTransitionTrigger("Contacted"), Contacted)
				.Transition(new ButtonTransitionTrigger("Meeting Scheduled"), MeetingScheduled)
				.Transition(new ButtonTransitionTrigger("Proposal Delivered"), ProposalDelivered)
				.End();
		}

		private void OnAssigning()
		{
		}
	}

	public class SampleStateModel : IFlowModel
	{ }
}
