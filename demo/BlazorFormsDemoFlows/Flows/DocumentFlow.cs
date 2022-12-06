using System.Collections.Generic;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;

namespace BlazorFormsDemoFlows
{
    public abstract class DocumentFlowBase<M> : StateFlowBase<M>
        where M : class, IFlowModel
    {
        public state New;
        public state Assigned;
        public state Reviewing;
        public state Alarmed;
        public state Closed;

        public status Open;
        public status Stale;
        public status StatusClosed = new status("Closed");

    }

    public class DocumentFlow : DocumentFlowBase<DocumentModel>
    {
        public override void Define()
        {
            this.State(New)
                .Transition(AssignTrigger, Assigned, OnAssigning)
                .Transition(DaySpanTrigger(1), Assigned, OnAssigning)
            .State(Assigned)
                .Transition(ReviewingTrigger, Reviewing, OnReviewing)
                .Transition(FastResolveTrigger, Closed, OnAssignClosing)
                .Transition(DaySpanTrigger(15), Alarmed, OnAlarming)
            .State(Reviewing)
                .Transition(ReturnAssignTrigger, Assigned, OnReturnAssigning)
                .Transition(CloseTrigger, Closed, OnClosing)
                .Transition(DaySpanTrigger(5), Alarmed, OnAlarming)
            .State(Alarmed)
                .Transition(CloseTrigger, Closed, OnClosing)
            .State(Closed)
                .Transition(ReopenTrigger, Assigned, OnReopening)
                .End();
        }

        private TransitionTrigger AssignTrigger()
        {
            return new ButtonTransitionTrigger("Assign", new string[] { "ivan@satoshi.com", "john@satoshi.com", "michael@satoshi.com" });
        }
        private TransitionTrigger ReturnAssignTrigger()
        {
            return new ButtonTransitionTrigger("Return", new string[] { "ivan@satoshi.com", "john@satoshi.com", "michael@satoshi.com" });
        }

        private TransitionTrigger FastResolveTrigger()
        {
            return new ButtonTransitionTrigger("Resolve", new string[] { "Ignore", "Report", "Rollback" });
        }

        private TransitionTrigger ReviewingTrigger()
        {
            return new ButtonTransitionTrigger("Reviewing");
        }

        private TransitionTrigger CloseTrigger()
        {
            return new ButtonTransitionTrigger("Close", new string[] { "Ignore", "Report", "Rollback" });
        }

        private TransitionTrigger ReopenTrigger()
        {
            return new ButtonTransitionTrigger("Open");
        }

        private void OnAssigning()
        {
            Status = Open;
            Model.AssignedUser = Model.TriggerSelectedValue;
        }
        private void OnReturnAssigning()
        {
            Status = Open;
            Model.AssignedUser = Model.TriggerSelectedValue;
        }

        private void OnAssignClosing()
        {
            Status = StatusClosed;
            Model.Resolution = Model.TriggerSelectedValue;
        }

        private void OnClosing()
        {
            Status = StatusClosed;
            Model.Resolution = Model.TriggerSelectedValue;
        }

        private void OnReviewing()
        {
            Status = Open;
        }

        private void OnReopening()
        {
            Status = Open;
        }

        private void OnAlarming()
        {
            Status = Stale;
        }
    }

    public class Document
    {
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public string AccountId { get; set; }
        public string FoundIssue { get; set; }
    }

    public class DocumentModel : FlowModelBase
    {
        public string RefId { get; set; }
        public string State { get; set; }
        public string StatusMessage { get; set; }
        public IFlowContext FlowContext { get; set; }
        public Document Document { get; set; }
        public bool Selected { get; set; }
        public string Resolution { get; set; }
        public string AssignedUser { get; set; }
        public string TriggerSelectedValue { get; set; }
    }
}
