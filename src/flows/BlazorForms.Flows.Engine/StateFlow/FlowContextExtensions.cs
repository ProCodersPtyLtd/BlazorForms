using BlazorForms.Flows.Definitions;

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
