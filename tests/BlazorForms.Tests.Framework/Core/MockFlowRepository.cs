﻿using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Tests.Framework.Core
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

        public IAsyncEnumerable<FlowEntity> GetFlowEntities<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteFlow(string tenantId, string flowName, string itemId, string refId)
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
