﻿using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorFormsDemoFlows
{
    public class MockFlowRepository : IFlowRepository
    {
        public IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
        {
            return null;
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

        public IAsyncEnumerable<FlowEntity> GetFlowEntities<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteFlow(string tenantId, string flowName, string itemId, string refId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity)
        {
            //throw new System.NotImplementedException();
            return null;
        }

        IAsyncEnumerable<(string, T)> IFlowRepository.GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            return null;
        }
    }
}
