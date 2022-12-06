using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public static class FlowContextExtensions
    {
        public static string GetState(this IFlowContext context)
        {
            return context.CurrentTask;
        }
    }
}
