using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorFormsDemoWasmNew.Server
{
    public class MockFlowRepository : IFlowRepository
    {
        public IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
        {
            throw new System.NotImplementedException();
        }  
        
        public IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId)
        {
            throw new System.NotImplementedException();
        }

        public Task<FlowEntity> GetFlowByRef(string tenantId, string refId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity)
        {
            throw new System.NotImplementedException();
        }

        IAsyncEnumerable<(string, T)> IFlowRepository.GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            throw new System.NotImplementedException();
        }
    }
}
