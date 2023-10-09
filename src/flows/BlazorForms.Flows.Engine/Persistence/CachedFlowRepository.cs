using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorForms.Flows.Definitions;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorForms.Flows.Engine.Persistence;

public interface ICachedFlowRepository : IFlowRepository
{
}

public class CachedFlowRepository : ICachedFlowRepository
{
    private readonly IFlowRepository _repo;
    private readonly IMemoryCache _cache;

    public CachedFlowRepository(IFlowRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<FlowEntity> GetFlowByRef(string tenantId, string refId)
    {
        var cacheKey = $"FlowByRef-{tenantId}-{refId}";
        if (_cache.TryGetValue(cacheKey, out FlowEntity flowEntity))
        {
            return flowEntity;
        }
        
        flowEntity = await _repo.GetFlowByRef(tenantId, refId);
        if (flowEntity != null)
        {
            _cache.Set(cacheKey, flowEntity);
        }

        return flowEntity;
    }

    public IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
    {
        // implement a pass-through to the repo
        return _repo.GetActiveFlowsIds(tenantId, flowName);
    }

    public IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId)
    {
        // implement a pass-through to the repo
        return _repo.GetAllWaitingFlowsIds(tenantId);
    }

    public IAsyncEnumerable<(string, T)> GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
    {
        // implement a pass-through to the repo
        return _repo.GetFlowModels<T>(tenantId, flowModelsQueryOptions);
    }
    
    public IAsyncEnumerable<FlowEntity> GetFlowEntities<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
    {
        return _repo.GetFlowEntities<T>(tenantId, flowModelsQueryOptions);
    }
    
    public async Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
    {
        // implement caching same way as other cached methods
        var cacheKey = $"FlowContexts-{tenantId}-{flowModelsQueryOptions.FlowName}";
        if (_cache.TryGetValue(cacheKey, out List<FlowContextJsonModel> flowContexts))
        {
            return flowContexts;
        }
        
        flowContexts = await _repo.GetFlowContexts(tenantId, flowModelsQueryOptions);
        _cache.Set(cacheKey, flowContexts);

        return flowContexts;
    }

    public async Task<bool> DeleteFlow(string tenantId, string flowName, string itemId, string refId)
    {
        var result = await _repo.DeleteFlow(tenantId, flowName, itemId, refId);
        if (!result)
        {
            return false;
        }
        
        var flowRefCacheKey = $"FlowByRef-{tenantId}-{refId}";
        _cache.Remove(flowRefCacheKey);

        // invalidate the FlowContexts cache
        var flowContextsCacheKey = $"FlowContexts-{tenantId}-{flowName}";
        _cache.Remove(flowContextsCacheKey);

        return true;
    }

    public async Task<string> UpsertFlow(string tenantId, FlowEntity entity)
    {
        var result = await _repo.UpsertFlow(tenantId, entity);
        var flowRefCacheKey = $"FlowByRef-{tenantId}-{entity.RefId}";
        _cache.Remove(flowRefCacheKey);
        
        // invalidate the FlowContexts cache
        var flowContextsCacheKey = $"FlowContexts-{tenantId}-{entity.FlowName}";
        _cache.Remove(flowContextsCacheKey);
        
        return result;
    }
}