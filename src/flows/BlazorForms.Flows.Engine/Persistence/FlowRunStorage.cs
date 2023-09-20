using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorForms.Flows.Definitions;

namespace BlazorForms.Flows.Engine.Persistence
{
    public class FlowRunStorage : IFlowRunStorage
    {
        private readonly ICachedFlowRepository _repo;
        private readonly IObjectCloner _cloner;
        private readonly ITenantedScope _tenantedScope;

        public FlowRunStorage(ICachedFlowRepository repo, IObjectCloner cloner, ITenantedScope tenantedScope)
        {
            _repo = repo;
            _cloner = cloner;
            _tenantedScope = tenantedScope;
        }

        public async Task<T> CloneObject<T>(T source)
        {
            return await _cloner.CloneObject(source);
        }

        public async Task WarmUp()
        {
        }


        public async Task<IFlowContext> CreateProcessExecutionContext(IFlow flow, FlowParamsGeneric parameters, bool noStorage = false)
        {
            var context = flow.CreateContext();
            context.FlowAssembly = flow.GetType().Assembly.GetName().Name;
            context.FlowName = flow.GetType().FullName;
            context.Params = parameters;
            context.AssignedUser = parameters?.AssignedUser;
            context.AssignedTeam = parameters?.AssignedTeam;
            context.ExecutionResult = new TaskExecutionResult();

            var entity = new FlowEntity
            {
                Created = DateTime.UtcNow,
                RefId = context.RefId,
                FlowName = context.FlowName,
                FlowTags = context.FlowTags,
                Context = context as FlowContext
            };

            if (noStorage || (flow.Settings.StoreModel != FlowExecutionStoreModel.Full &&
                              flow.Settings.StoreModel != FlowExecutionStoreModel.FullNoHistory))
            {
                return context;
            }

            var tId = await _tenantedScope.GetTenantId();
            context.Id = await _repo.UpsertFlow(tId, entity);

            return context;
        }

        public async IAsyncEnumerable<string> GetActiveFlowsIds(string flowName)
        {
            var tId = await _tenantedScope.GetTenantId();
            await foreach (var i in _repo.GetActiveFlowsIds(tId, flowName))
            {
                yield return i;
            }
        }       
        
        public async IAsyncEnumerable<string> GetAllWaitingFlowsIds()
        {
            var tId = await _tenantedScope.GetTenantId();
            await foreach (var i in _repo.GetAllWaitingFlowsIds(tId))
            {
                yield return i;
            }
        }

        public async Task<IFlowContext> GetProcessExecutionContext(string refId)
        {
            var tId = await _tenantedScope.GetTenantId();
            var details = await _repo.GetFlowByRef(tId, refId);
            var last = details?.Context;
            return last;
        }

        public async Task SaveProcessExecutionContext(IFlowContext context, TaskExecutionResult result, bool create = false)
        {
            if (result != null)
            {
                context.ExecutionResult = result;
            }

            var tId = await _tenantedScope.GetTenantId();
            FlowEntity flowEntity;

            if (create)
            {
                flowEntity = new FlowEntity
                {
                    Created = DateTime.UtcNow,
                    RefId = context.RefId,
                    FlowName = context.FlowName,
                    FlowTags = context.FlowTags,
                };
            }
            else
            {                
                flowEntity = await _repo.GetFlowByRef(tId, context.RefId);
            }

            flowEntity.Context = context as FlowContext;
            flowEntity.RefId = context.RefId;
            flowEntity.FlowTags = context.FlowTags;

            if (context.ExecutionResult.FlowState == TaskExecutionFlowStateEnum.Finished)
            {
                flowEntity.FlowStatus = FlowStatus.Finished;
            }

            context.Id = await _repo.UpsertFlow(tId, flowEntity);
        }

        public async Task SetProcessFlowStatus(string refId, FlowStatus flowStatus)
        {
            var tId = await _tenantedScope.GetTenantId();
            var flow = await _repo.GetFlowByRef(tId, refId);
            flow.FlowStatus = flowStatus;
            await _repo.UpsertFlow(tId, flow);
        }
 
        public async Task<IEnumerable<IFlowContext>> GetProcessFlowHistory(string refId)
        {
            throw new NotImplementedException("Flow History is not required by FluentFlow and currently not implemented");
            //var flow = await _repo.GetFlowByRefId(refId);
            //return flow.Records;
        }

        public async Task SaveProcessExecutionContextHistory(IEnumerable<IFlowContext> contextHistory)
        {
            throw new NotImplementedException("Flow History is not required by FluentFlow and currently not implemented");
        }

        public async Task<FlowEntity> GetFlowByRef(string refId)
        {
            var tId = await _tenantedScope.GetTenantId();
            var flowEntity = await _repo.GetFlowByRef(tId, refId);
            return flowEntity;
        }

        public async Task<IFlowModel> GetFlowModelByRef(string refId)
        {
            var tId = await _tenantedScope.GetTenantId();
            var model = await _repo.GetFlowByRef(tId, refId);
            return model?.Context?.Model;
        }

        public async IAsyncEnumerable<(string, T)> GetFlowModels<T>(FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
        {
            var tId = await _tenantedScope.GetTenantId();
            await foreach (var i in _repo.GetFlowModels<T>(tId, flowModelsQueryOptions))
            {
                yield return i;
            }
        }

        public async Task<List<FlowContextJsonModel>> GetFlowContexts(FlowModelsQueryOptions flowModelsQueryOptions)
        {
            var tId = await _tenantedScope.GetTenantId();
            var data = await _repo.GetFlowContexts(tId, flowModelsQueryOptions);
            return data;
        }
    }
}
