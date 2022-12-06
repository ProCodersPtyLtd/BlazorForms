using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Wasm.InlineFlows
{
    public class MockFlowRepository : IFlowRepository
    {
        public IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
        {
            throw new NotImplementedException("MockFlowRepository can be used only for NoStorage flow run mode");
        }

        public IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId)
        {
            throw new NotImplementedException();
        }

        public async Task<FlowEntity> GetFlowByRef(string tenantId, string refId)
        {
            throw new NotImplementedException("MockFlowRepository can be used only for NoStorage flow run mode");
        }

        public Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity)
        {
            throw new NotImplementedException("MockFlowRepository can be used only for NoStorage flow run mode");
        }

        IAsyncEnumerable<(string, T)> IFlowRepository.GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            throw new NotImplementedException("MockFlowRepository can be used only for NoStorage flow run mode");
        }
    }
}
