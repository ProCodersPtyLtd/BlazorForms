using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class SpanTransitionTrigger : TransitionTrigger
    {
        public DateSpan DateSpan { get; set; }

        public SpanTransitionTrigger(DateSpan span)
        {
            DateSpan = span;
        }

        public override void CheckTrigger(IFlowContext context)
        {
            var duration = DateTime.Now - context.ExecutionResult.CreatedDate;

            if (duration > DateSpan.Value)
            {
                Proceed = true;
            }
        }
    }
}
