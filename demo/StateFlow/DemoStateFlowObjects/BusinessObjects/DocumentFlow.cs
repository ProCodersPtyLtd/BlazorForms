using System.Collections.Generic;
using BlazorForms.Flows;
using BlazorForms.Shared;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public class DocumentFlow : DocumentFlowBase<DocumentModel>
    {
        private readonly IDocumentApi _api;

        public DocumentFlow(IDocumentApi api)
        {
            _api = api;
        }

        public override void Define()
        {
            this.State(New)
                .Transition(AutoAssignTrigger, Assigned)
                .Transition(AssignTrigger, Assigned, OnAssigning)
                .Transition(new WorkingDaysCustomTransitionTrigger(1), Assigned, OnAssigning)
            .State(Assigned)
                .Transition(ReviewingTrigger, Reviewing, OnReviewing)
                .Transition(FastResolveTrigger, Closed, OnAssignClosing)
                .Transition(DaySpanTrigger(15), Alarmed, OnAlarming)
                .Transition(AutoDataChangedReviewingTrigger, Reviewing)
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

        private TransitionTrigger AutoAssignTrigger()
        {
            return new ConditionTransitionTrigger((context) => 
            {
                if (Model.Document.CreatedUser == "tcra")
                {
                    Model.AssignedUser = "owner1@satoshi.com";
                    return true;
                }

                return false;
            });
        }

        private TransitionTrigger AutoDataChangedReviewingTrigger()
        {
            return new ConditionTransitionTrigger((context) =>
            {
                var current = new ModelProperies();
                current.List.Add(new ModelPropertyDetails { Id = Model.Document.TransactionId, Bindnig = "AssignedUser", Value = Model.AssignedUser });
                var changes = _api.GetModelChanges(current);

                if (changes.Any())
                {
                    return true;
                }

                return false;
            });
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
}
