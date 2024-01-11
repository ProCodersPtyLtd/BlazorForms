using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Flows.Definitions
{
    public interface IFlowRepository
    {
        Task<bool> DeleteFlow(string tenantId, string flowName, string itemId, string refId);
        Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity);
        Task<FlowEntity> GetFlowByRef(string tenantId, string refId);
        IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName);
        IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId);
        IAsyncEnumerable<(string, T)> GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel;
        Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions);
        IAsyncEnumerable<FlowEntity> GetFlowEntities<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel;
    }
}
