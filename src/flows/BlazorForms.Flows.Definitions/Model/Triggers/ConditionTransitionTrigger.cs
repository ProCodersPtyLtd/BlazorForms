using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class ConditionTransitionTrigger : TransitionTrigger
    {
        private Func<IFlowContext, bool> _condition;
        private Func<IFlowContext, Task<bool>> _conditionAsync;

        public ConditionTransitionTrigger(Func<IFlowContext, bool> condition)
        {
            _condition = condition;
        }

        public ConditionTransitionTrigger(Func<IFlowContext, Task<bool>> condition)
        {
            _conditionAsync = condition;
        }

        public override bool IsTriggerAsync()
        {
            return _conditionAsync != null;
        }

        public override void CheckTrigger(IFlowContext context)
        {
            if (_condition(context))
            {
                Proceed = true;
            }
        }

        public override async Task CheckTriggerAsync(IFlowContext context)
        {
            if (await _conditionAsync(context))
            {
                Proceed = true;
            }
        }
    }
}
