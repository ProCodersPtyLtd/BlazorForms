using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Engine.StateFlow
{
    public interface IStateFlowRunEngine : IFlowRunEngine
    {
        Task<StateFlowTaskDetails> GetStateDetails(FlowRunParameters runParameters);
        Task<IFlowContext> ContinueFlow(string refId, IFlowModel model, string operationName = null, FlowParamsGeneric flowParams = null);
    }
}
