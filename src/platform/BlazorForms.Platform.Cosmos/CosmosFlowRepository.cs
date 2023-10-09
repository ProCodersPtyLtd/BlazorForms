using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Platform.Cosmos.Configuration;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorForms.Platform.Cosmos;

public class CosmosFlowRepository : IFlowRepository
{
    private readonly IKnownTypesBinder _knownTypesBinder;
    private readonly BfCosmosSerializer _serializer;
    private readonly ILogger _logger;
    private readonly ILogStreamer _logStreamer;
    private readonly CosmosDbOptions _cosmosDbOptions;
    private readonly IMapper _mapper;

    public CosmosFlowRepository(
        IKnownTypesBinder knownTypesBinder,
        BfCosmosSerializer serializer,
        ILogger<CosmosFlowRepository> logger,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        ILogStreamer logStreamer, IMapper mapper)
    {
        _cosmosDbOptions = cosmosDbOptions.Value;
        _knownTypesBinder = knownTypesBinder;
        _serializer = serializer;
        _logger = logger;
        _logStreamer = logStreamer;
        _mapper = mapper;

        if (string.IsNullOrEmpty(_cosmosDbOptions.Database)
            || string.IsNullOrEmpty(_cosmosDbOptions.Uri)
            || string.IsNullOrEmpty(_cosmosDbOptions.Key)
            || string.IsNullOrEmpty(_cosmosDbOptions.EnvironmentTag))
        {
            throw new ArgumentException("Not all required CosmosDB settings provided.", nameof(cosmosDbOptions));
        }

        _cosmosDbOptions.FlowCollection ??= DefaultFlowCollection;

        GetOrCreateDatabase(_cosmosDbOptions.Database)
            .ConfigureAwait(true)
            .GetAwaiter()
            .GetResult();
    }

    #region Schema

    private CosmosClient _client; //=> _clientLazy.Value.Result;
    private Database _database;

    private const int DefaultThroughput = 400;
    private const string DefaultFlowCollection = "_cosmosDbOptions.FlowCollection";

    private Container _container;
    
    private async Task GetOrCreateDatabase(string id)
    {
        _client = new CosmosClient(_cosmosDbOptions.Uri, _cosmosDbOptions.Key, new CosmosClientOptions
        {
            Serializer = _serializer
        });

        // Get the database by name, or create a new one if one with the name provided doesn't exist.
        // Create a query object for database, filter by name.
        _database = await _client.CreateDatabaseIfNotExistsAsync(id, 
            ThroughputProperties.CreateAutoscaleThroughput(DefaultThroughput));

        var containerProperties = new ContainerProperties
        {
            Id = _cosmosDbOptions.FlowCollection,
            PartitionKeyPath = "/RefId",
            IndexingPolicy = new IndexingPolicy
            {
                IndexingMode = IndexingMode.Consistent,
                IncludedPaths = {
                    new IncludedPath { Path = "/EnvTag/?" },
                    new IncludedPath { Path = "/Version/?" },
                    new IncludedPath { Path = "/RefId/?" },
                    new IncludedPath { Path = "/TenantId/?" },
                    new IncludedPath { Path = "/Created/?" },
                    new IncludedPath { Path = "/FlowName/?" },
                    new IncludedPath { Path = "/DocumentType/?" },
                    new IncludedPath { Path = "/FlowTags/*" },
                    new IncludedPath { Path = "/FlowStatus/?" },
                    new IncludedPath { Path = "/Context/Model/*" }
                },
                ExcludedPaths =
                {
                    new ExcludedPath { Path = "/*" }
                }
            }
        };
        
        _container = await _database.CreateContainerIfNotExistsAsync(containerProperties);
    }
    #endregion

    public async Task<bool> DeleteFlow(string tenantId, string flowName, string itemId, string refId)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var result = await _container.DeleteItemAsync<FlowEntity>(itemId, new PartitionKey(refId));
            return (int)result.StatusCode >= 200 && (int)result.StatusCode <= 299;
        }
        catch (Exception exc)
        {
            _logStreamer.TrackException(exc);
            throw;
        }
        finally
        {
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(DeleteFlow), elapsedMs);
        }
    }

    public async Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            flowEntity.id ??= Guid.NewGuid().ToString();
            flowEntity.TenantId = tenantId ?? flowEntity.TenantId;
            flowEntity.EnvTag = _cosmosDbOptions.EnvironmentTag;
            var result = await _container.UpsertItemAsync(flowEntity);
            return result.Resource.id;
        }
        catch (Exception exc)
        {
            _logStreamer.TrackException(exc);
            throw;
        }
        finally
        {
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(UpsertFlow), elapsedMs);
        }
    }

    public async Task<FlowEntity> GetFlowByRef(string tenantId, string refId)
    {
        FlowEntity result = null;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var queryable = _container.GetItemLinqQueryable<FlowEntity>();
            var query = queryable.Where(f => f.RefId == refId);

            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(f => f.TenantId == tenantId);
            }

            query = query.Take(1);

            // Convert to feed iterator
            using var linqFeed = query.ToFeedIterator();
                
            // Iterate query result
            while (linqFeed.HasMoreResults)
            {
                var response = await linqFeed.ReadNextAsync();
                result = response.FirstOrDefault();
            }
        }
        finally
        {
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(GetFlowByRef), elapsedMs);
        }

        return result;
    }

    public async IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var queryable = _container.GetItemLinqQueryable<FlowEntity>();
        var query = queryable
            .Where(f => f.EnvTag == _cosmosDbOptions.EnvironmentTag &&
                        f.FlowStatus != FlowStatus.Deleted && f.FlowStatus != FlowStatus.Finished &&
                        f.FlowName == flowName);

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(f => f.TenantId == tenantId);
        }

        // Convert to feed iterator
        using var iterator = query
            .Select(f => f.RefId)
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                yield return item;
            }
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(GetActiveFlowsIds), elapsedMs);
    }

    public async IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var queryable = _container.GetItemLinqQueryable<FlowEntity>();
        var query = queryable
            .Where(f => f.EnvTag == _cosmosDbOptions.EnvironmentTag &&
                        f.Context.ExecutionResult.IsWaitTask == true && 
                        f.FlowStatus != FlowStatus.Deleted &&
                        f.FlowStatus != FlowStatus.Finished);

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(f => f.TenantId == tenantId);
        }

        using var iterator = query
            .Select(f => f.RefId)
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                yield return item;
            }
        }
        
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(GetAllWaitingFlowsIds), elapsedMs);
    }

    private struct FlowIdModel
    {
        public string RefId;
        public dynamic Model;
    }

    private (string RefId, T) CastToFlowModel<T>(FlowIdModel p) where T : class, IFlowModel
    {
        try
        {
            // TODO: Confirm this is the best performance option
            var mapped = _mapper.Map<T>(p.Model);
            return (p.RefId, mapped);
        }
        catch (Exception ex)
        {
            // _logger.LogWarning("Failed to deserialize flow RefId {RefId}, {ex}", p.RefId, ex);
        }

        return (p.RefId, null);
    }

    public async IAsyncEnumerable<FlowEntity> GetFlowEntities<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        
        flowModelsQueryOptions.FlowStatuses ??= new List<FlowStatus>
        {
            FlowStatus.Created, FlowStatus.Started, FlowStatus.Waiting, FlowStatus.Failed
        };

        var queryable = _container.GetItemLinqQueryable<FlowEntity>();
        var query = queryable
            .Where(f => f.EnvTag == _cosmosDbOptions.EnvironmentTag)
            .Where(f => f.FlowStatus != FlowStatus.Deleted &&
                        flowModelsQueryOptions.FlowStatuses.Contains(f.FlowStatus))
            .Where(f => f.Context != null && f.Context.Model != null);

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(f => f.TenantId == tenantId);
        }

        if (!string.IsNullOrEmpty(flowModelsQueryOptions.FlowName))
        {
            query = query.Where(f => f.FlowName == flowModelsQueryOptions.FlowName);
        }

        if (flowModelsQueryOptions.Tags != null && flowModelsQueryOptions.Tags.Any())
        {
            if (flowModelsQueryOptions.SearchAnyTag)
            {
                query = query.Where(f => f.FlowTags.Count(ft => flowModelsQueryOptions.Tags.Contains(ft)) > 0);
            }
            else
            {
                query = query.Where(f =>
                    f.FlowTags.Count(ft => flowModelsQueryOptions.Tags.Contains(ft)) ==
                    flowModelsQueryOptions.Tags.Count());
            }
        }

        if (flowModelsQueryOptions.RefIds != null && flowModelsQueryOptions.RefIds.Any())
        {
            query = query.Where(f => flowModelsQueryOptions.RefIds.Contains(f.RefId));
        }

        if (flowModelsQueryOptions.QueryOptions?.AllowFiltering == true)
        {
            query = QueryOptionsFilterHelper.ApplyFilters(query, flowModelsQueryOptions.QueryOptions, typeof(T));
        }

        if (flowModelsQueryOptions.QueryOptions?.AllowSort == true)
        {
            query = QueryOptionsSortHelper.OrderBy(query, flowModelsQueryOptions.QueryOptions, typeof(T));
        }
        else
        {
            query = query.OrderByDescending(f => f.Created);
        }

        if (flowModelsQueryOptions.QueryOptions?.AllowPagination == true)
        {
            query = QueryOptionsPaginationHelper.GetPaginationResult(query, flowModelsQueryOptions.QueryOptions);
        }

        var selector = query;
        using var iterator = selector.ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                yield return item;
            }
        }
        
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(GetFlowEntities), elapsedMs);
    }

    public async IAsyncEnumerable<(string, T)> GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        await foreach (var item in GetFlowEntities<T>(tenantId, flowModelsQueryOptions))
        {
            yield return CastToFlowModel<T>(new FlowIdModel { RefId = item.RefId, Model = item.Context.Model });
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        _logger.LogInformation("[{Method}] Elapsed {elapsedMs} ms", nameof(GetFlowModels), elapsedMs);
    }

    public async Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
    {
        flowModelsQueryOptions.FlowStatuses ??= new List<FlowStatus>
            { FlowStatus.Created, FlowStatus.Started, FlowStatus.Waiting, FlowStatus.Failed };

        var queryable = _container.GetItemLinqQueryable<FlowEntity>();
        var query = queryable
            .Where(f => f.EnvTag == _cosmosDbOptions.EnvironmentTag)
            .Where(f => f.FlowStatus != FlowStatus.Deleted &&
                        flowModelsQueryOptions.FlowStatuses.Contains(f.FlowStatus))
            .Where(f => f.Context != null && f.Context.Model != null);

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(f => f.TenantId == tenantId);
        }

        if (!string.IsNullOrEmpty(flowModelsQueryOptions.FlowName))
        {
            query = query.Where(f => f.FlowName == flowModelsQueryOptions.FlowName);
        }

        if (flowModelsQueryOptions.Tags != null && flowModelsQueryOptions.Tags.Any())
        {
            if (flowModelsQueryOptions.SearchAnyTag)
            {
                query = query.Where(f => f.FlowTags.Count(ft => flowModelsQueryOptions.Tags.Contains(ft)) > 0);
            }
            else
            {
                query = query.Where(f =>
                    f.FlowTags.Count(ft => flowModelsQueryOptions.Tags.Contains(ft)) ==
                    flowModelsQueryOptions.Tags.Count());
            }
        }

        if (flowModelsQueryOptions.RefIds != null && flowModelsQueryOptions.RefIds.Any())
        {
            query = query.Where(f => flowModelsQueryOptions.RefIds.Contains(f.RefId));
        }

        if (flowModelsQueryOptions.QueryOptions?.AllowFiltering == true)
        {
            query = QueryOptionsFilterHelper.ApplyFilters(query, flowModelsQueryOptions.QueryOptions);
        }

        if (flowModelsQueryOptions.QueryOptions?.AllowSort == true)
        {
            query = QueryOptionsSortHelper.OrderBy(query, flowModelsQueryOptions.QueryOptions);
        }
        else
        {
            query = query.OrderByDescending(f => f.Created);
        }

        if (flowModelsQueryOptions.QueryOptions?.AllowPagination == true)
        {
            query = QueryOptionsPaginationHelper.GetPaginationResult(query, flowModelsQueryOptions.QueryOptions);
        }

            
        var result = new List<FlowContextJsonModel>();
        var jsons = new List<JObject>();

        var selector = query.Select(f => f.Context);

        using var iterator = selector.ToFeedIterator();
            
        while (iterator.HasMoreResults)
        {
            jsons.AddRange((await iterator.ReadNextAsync()).Select(JObject.FromObject));
        }
            
        foreach (var json in jsons)
        {
            try
            {
                var modelTypeName = json.SelectToken("$.Model.$type").ToString();
                FlowContextJsonModel context;

                if (_knownTypesBinder.KnownTypesDict.ContainsKey(modelTypeName))
                {
                    context = JsonConvert.DeserializeObject<FlowContextJsonModel>(json.ToString());
                }
                else
                {
                    // Model cannot be deserialized, use null model
                    var contextNoModel = JsonConvert.DeserializeObject<FlowContextNoModel>(json.ToString());
                    context = new FlowContextJsonModel(contextNoModel, null);
                }

                var jsonModel = json.SelectToken("$.Model");
                context.ModelJson = jsonModel.ToString();
                context.ModelType = modelTypeName;
                result.Add(context);
            }
            catch (Exception exc)
            {

            }
        }

        return result;
    }
}

public class BfCosmosSerializer : CosmosSerializer
{
    private readonly JsonSerializerSettings _settings;
    
    public BfCosmosSerializer(IKnownTypesBinder knownTypesBinder)
    {
        _settings = new JsonSerializerSettings
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Objects,
            SerializationBinder = knownTypesBinder
        };
    }

    public override T FromStream<T>(Stream stream)
    {
        if (stream == null || stream.Length == 0)
        {
            return default;
        }

        using var sr = new StreamReader(stream);
        using var reader = new JsonTextReader(sr);
            
        var serializer = JsonSerializer.Create(_settings);
        return serializer.Deserialize<T>(reader);
    }

    public override Stream ToStream<T>(T input)
    {
        var stream = new MemoryStream();
        using var sw = new StreamWriter(stream, new UTF8Encoding(), 1024, true);
        using var writer = new JsonTextWriter(sw);

        var serializer = JsonSerializer.Create(_settings);
        serializer.Serialize(writer, input);
        writer.Flush();
            
        stream.Position = 0;
        return stream;
    }
}