using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowRunStorage
    {
        Task<IFlowContext> CreateProcessExecutionContext(IFlow flow, FlowParamsGeneric parameters, bool noStorage = false);
        Task SaveProcessExecutionContext(IFlowContext context, TaskExecutionResult result, bool create = false);
        Task SaveProcessExecutionContextHistory(IEnumerable<IFlowContext> contextHistory);
        //Task<IEnumerable<IFlowContext>> GetProcessFlowHistory(int flowRunId);
        Task<IEnumerable<IFlowContext>> GetProcessFlowHistory(string refId);
        //Task<IFlowContext> GetProcessExecutionContext(int flowRunId);
        Task<IFlowContext> GetProcessExecutionContext(string refId);
        //Task<IFlowContext> GetProcessExecutionContext(string refId, string taskName, int callIndex);
        IAsyncEnumerable<string> GetActiveFlowsIds(string flowName);
        IAsyncEnumerable<string> GetAllWaitingFlowsIds();
        Task<T> CloneObject<T>(T source);
        Task<FlowEntity> GetFlowByRef(string refId);
        Task SetProcessFlowStatus(string refId, FlowStatus flowStatus);
        Task WarmUp();
        Task<IFlowModel> GetFlowModelByRef(string refId);

        Task<bool> DeleteFlow(string refId);
        IAsyncEnumerable<(string, T)> GetFlowModels<T>(FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel;
        IAsyncEnumerable<FlowEntity> GetFlowEntities<T>(FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel;
        Task<List<FlowContextJsonModel>> GetFlowContexts(FlowModelsQueryOptions flowModelsQueryOptions);
    }
}
