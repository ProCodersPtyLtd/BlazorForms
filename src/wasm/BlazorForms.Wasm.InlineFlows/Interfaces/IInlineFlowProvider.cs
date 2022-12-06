using Microsoft.AspNetCore.Components;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Wasm.InlineFlows
{
    public interface IInlineFlowProvider<T> : IInlineFlowProvider where T : class, IFlowModel
    {
        T Model { get; }
    }

    public interface IInlineFlowProvider
    {
        IFlowModel? ModelUntyped { get; }
        IFlowParams? Params { get; }
        IFlowContextNoModel? Context { get; }

        Task InitiateFlow(string flowName, string refId, string pk);
        Task SubmitForm(string binding = null, string operationName = null);
    }
}
