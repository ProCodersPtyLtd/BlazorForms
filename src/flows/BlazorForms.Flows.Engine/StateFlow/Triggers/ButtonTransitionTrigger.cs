using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows 
{
    public class ButtonTransitionTrigger : TransitionTrigger
    {
        public StateFlowTransitionSelector Selector { get; set; }

        public ButtonTransitionTrigger()
        { }

        public ButtonTransitionTrigger(string text)
        {
            Text = text;
        }

        public ButtonTransitionTrigger(string text, IEnumerable<string> selectorValues)
        {
            Text = text;
            Selector = new StateFlowTransitionSelector(selectorValues);
        }

        public override void CheckTrigger(IFlowContext context)
        {
            // User pressed button with this trigger Text
            if (context.ExecutionResult.FormLastAction == Text)
            {
                Proceed = true;
            }
        }

        public override StateFlowTransitionSelector GetSelector()
        {
            return Selector;
        }

        public void SetSelector(IEnumerable<string> strings)
        {
            Selector = new StateFlowTransitionSelector(strings);
        }
    }
}
