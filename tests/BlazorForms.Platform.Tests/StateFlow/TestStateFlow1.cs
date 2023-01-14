using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Tests.StateFlow
{
    public class TestStateFlow1 : StateFlowBase<TestModel1>
    {
        public state Unassigned;
        public state Assigned;
        public state BeingReviewed;
        public state Escalated;
        public state Closed;

        public status NA;
        public status Open;
        public status StatusClosed;

        public override void Define()
        {
            this.State(Unassigned)
                .Transition(AssignTrigger, Assigned, OnAssigning)
            .State(Assigned)
                .Transition(RecoveryTrigger, BeingReviewed)
                .Transition(ReviewTrigger, Closed)
            .State(BeingReviewed)
                .Transition(CloseTrigger, Closed)
            .State(Closed)
                .Transition(ReopenTrigger, Assigned)
                .End();
        }

        private TransitionTrigger AssignTrigger()
        {
            var t = new ButtonTransitionTrigger("Assign");
            t.SetSelector(GetAssignedUsersRoles());
            return t;
        }

        private List<string> GetAssignedUsersRoles()
        {
            return new List<string>(new string[] { "ivan.de@satoshi.com", "john.ze@satoshi.com" });
        }

        private TransitionTrigger ReviewTrigger()
        {
            return new ButtonTransitionTrigger("Review");
        }

        private TransitionTrigger RecoveryTrigger()
        {
            return new ButtonTransitionTrigger("Recover");
        }

        private TransitionTrigger CloseTrigger()
        {
            return new ButtonTransitionTrigger("Close");
        }

        private TransitionTrigger ReopenTrigger()
        {
            return new ButtonTransitionTrigger("Reopen");
        }

        private void OnAssigning()
        {
            Status = Open;
            AssignedUser = "ivan.de@satoshi.com";
            AssignedRole = "Level1Support";
        }
    }

    public class TestModel1 : FlowModelBase
    {
    }
}


