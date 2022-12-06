using Microsoft.AspNetCore.Components;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Wasm.InlineFlows
{
    public class InlineFlowProvider : IInlineFlowProvider
    {
        public IFlowModel? ModelUntyped { get; protected set; }
        public IFlowParams? Params { get; protected set; }
        public IFlowContextNoModel? Context { get; protected set; }

        protected readonly IFlowRunEngine _engine;
        protected readonly NavigationManager _navigationManager;

        public InlineFlowProvider(IFlowRunEngine engine, NavigationManager navigationManager)
        {
            _engine = engine;
            _navigationManager = navigationManager;
        }

        public async Task InitiateFlow(string flowName, string refId, string pk)
        {
            if (string.IsNullOrEmpty(flowName))
            {
                throw new Exception("Flow name must be supplied");
            }

            var flowParams = new FlowParamsGeneric { ItemId = pk };
            flowParams["BaseUri"] = _navigationManager.BaseUri;
            Params = flowParams;

            var ctx = await _engine.ExecuteFlowNoStorage(flowName, flowParams);
            Context = ctx.GetClientContext();
            ModelUntyped = ctx.Model;
        }

        public async Task SubmitForm(string binding = null, string operationName = null)
        {
            var parameters = Params as FlowParamsGeneric;
            var context = new FlowContext(Context, ModelUntyped);
            context.CallStack.Add(context.CurrentTask);
            context.Model = ModelUntyped;
            var result = context.ExecutionResult;
            result.FormState = FormTaskStateEnum.Submitted;
            result.FormLastAction = binding;

            var resultContext = await _engine.ContinueFlowNoStorage(context, parameters?.OperationName, parameters);

            Context = resultContext;
            ModelUntyped = resultContext.Model;
        }
    }
}
