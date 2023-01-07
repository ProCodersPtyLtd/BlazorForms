using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows 
{
    public class UserActionTransitionTrigger : TransitionTrigger
    {
		public UserActionTransitionTrigger()
        { }

        public UserActionTransitionTrigger(state state)
        {
            Text = state.Caption;
            CommandText = state.Value;
        }

        public override void CheckTrigger(IFlowContext context)
        {
            // User pressed button with this trigger Text
            if (context.ExecutionResult.FormLastAction == CommandText)
            {
                Proceed = true;
            }
        }

        public override StateFlowTransitionSelector GetSelector()
        {
            return null;
        }

        public void SetSelector(IEnumerable<string> strings)
        {
        }
    }
}
