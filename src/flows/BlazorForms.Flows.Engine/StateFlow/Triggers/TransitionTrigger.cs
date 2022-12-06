using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class TransitionTrigger
    {
        public string Text { get; set; }
        public bool Proceed { get; set; }

        public virtual void CheckTrigger(IFlowContext context)
        {
        }

        public virtual async Task CheckTriggerAsync(IFlowContext context)
        {
        }

        public virtual bool IsTriggerAsync()
        {
            return false;
        }

        public virtual StateFlowTransitionSelector GetSelector()
        {
            return null;
        }
    }
}
