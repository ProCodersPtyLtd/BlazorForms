using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class SpanFromChangeTransitionTrigger : TransitionTrigger
    {
        public DateSpan DateSpan { get; set; }

        public SpanFromChangeTransitionTrigger(DateSpan span)
        {
            DateSpan = span;
        }

        public override void CheckTrigger(IFlowContext context)
        {
            var duration = DateTime.Now - context.ExecutionResult.ChangedDate;

            if (duration > DateSpan.Value)
            {
                Proceed = true;
            }
        }
    }
}
