using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Tests.Framework.Core
{
    public class MockFlowRunStorage : IFlowRunStorage
    {
        public Task<T> CloneObject<T>(T source)
        {
            throw new NotImplementedException();
        }

        public Task<IFlowContext> CreateProcessExecutionContext(IFlow flow, FlowParamsGeneric parameters, bool noStorage = false)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<string> GetActiveFlowsIds(string flowName)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<string> GetAllWaitingFlowsIds()
        {
            throw new NotImplementedException();
        }

        public Task<FlowEntity> GetFlowByRef(string refId)
        {
            throw new NotImplementedException();
        }

        public Task<List<FlowContextJsonModel>> GetFlowContexts(FlowModelsQueryOptions flowModelsQueryOptions)
        {
            throw new NotImplementedException();
        }

        public Task<IFlowModel> GetFlowModelByRef(string refId)
        {
            throw new NotImplementedException();
        }

        public Task<IFlowContext> GetProcessExecutionContext(string refId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IFlowContext>> GetProcessFlowHistory(string refId)
        {
            throw new NotImplementedException();
        }

        public Task SaveProcessExecutionContext(IFlowContext context, TaskExecutionResult result, bool create = false)
        {
            throw new NotImplementedException();
        }

        public Task SaveProcessExecutionContextHistory(IEnumerable<IFlowContext> contextHistory)
        {
            throw new NotImplementedException();
        }

        public Task SetProcessFlowStatus(string refId, FlowStatus flowStatus)
        {
            throw new NotImplementedException();
        }

        public Task WarmUp()
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<(string, T)> IFlowRunStorage.GetFlowModels<T>(FlowModelsQueryOptions flowModelsQueryOptions)
        {
            throw new NotImplementedException();
        }
    }
}
