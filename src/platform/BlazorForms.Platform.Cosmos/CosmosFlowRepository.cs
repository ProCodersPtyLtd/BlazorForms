using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BlazorForms.Flows;
using Newtonsoft.Json.Linq;

namespace BlazorForms.Platform.ProcessFlow
{
    public class CosmosFlowRepository: IFlowRepository
    {
        private readonly ILogger _logger;
        private readonly IKnownTypesBinder _knownTypesBinder;
        private readonly ILogStreamer _logStreamer;

        public CosmosFlowRepository(
            IKnownTypesBinder knownTypesBinder,
            ILogger<CosmosFlowRepository> logger,
            IConfiguration configuration,
            ILogStreamer logStreamer)
        {
            _logger = logger;
            _logStreamer = logStreamer;
            _knownTypesBinder = knownTypesBinder;

            //var cosmos = config.GetSection("CosmosDb");
            var cosmos = configuration.GetSection("CosmosDb");

            if (cosmos != null)
            {
                _flowDatabase = cosmos["Database"] ?? _flowDatabase;
                _databaseUri = cosmos["Uri"] ?? _databaseUri;
                _databaseKey = cosmos["Key"] ?? _databaseKey;
                _environmentTag = string.IsNullOrEmpty(cosmos["EnvironmentTag"]) ? _environmentTag : cosmos["EnvironmentTag"];
            }

            GetOrCreateDatabase(_flowDatabase);
        }

        #region Schema
        private DocumentClient _client; //=> _clientLazy.Value.Result;
        private Database _database;

        private const int DEFAULT_THROUGHPUT = 400;
        // local emulator - default
        private readonly string _flowDatabase = "platform";
        private readonly string _databaseUri = "https://localhost:8081";
        private readonly string _databaseKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly string _environmentTag = Environment.MachineName;
        // ci
        //private readonly string _flowDatabase = "platform-ci";
        //private readonly string _databaseUri = "https://dev-cosmos.documents.azure.com:443/";
        //private readonly string _databaseKey = "8PvbcQkcHSfvMtNBC8skIfKuIPyE9dsPKr5PjVgbGv46PqilX19gUJzdBOjxYklAjEic6J1EdeUEnHyk9feqQQ==";

        //private readonly string FormFlowCollection = "FormFlowCollection";
        private readonly string FlowCollection = DEFAULT_FLOW_COLLECTION;
        private static readonly string DEFAULT_FLOW_COLLECTION = "FlowCollection";
        private static string _flowDocType = FlowEntityTypes.Flow.GetDescription();


        //private DocumentCollection FormFlow => GetFormFlowCollection();

        private void GetOrCreateDatabase(string id)
        {
            var settigns = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _knownTypesBinder
            };

            _client = new DocumentClient(
                new Uri(_databaseUri), _databaseKey, settigns);

            // Get the database by name, or create a new one if one with the name provided doesn't exist.
            // Create a query object for database, filter by name.
            var feed = new FeedOptions { };
            IEnumerable<Database> query = from db in _client.CreateDatabaseQuery()
                                          where db.Id == id
                                          select db;

            // Run the query and get the database (there should be only one) or null if the query didn't return anything.
            // Note: this will run synchronously. If async exectution is preferred, use IDocumentServiceQuery<T>.ExecuteNextAsync.
            _database = query.FirstOrDefault();

            if (_database == null)
            {
                // Create the database.
                var options = new RequestOptions { OfferThroughput = DEFAULT_THROUGHPUT };
                _database = _client.CreateDatabaseAsync(new Database { Id = id }, options).Result;
            }

            GetFlowCollectionNonAsync();
        }

        private DocumentCollection GetFlowCollectionNonAsync()
        {
            try
            {
                return _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection)).Result;
            }
            catch (Exception e)
            {
                _logStreamer.TrackException(e);
                DocumentCollection myCollection = new DocumentCollection
                {
                    Id = FlowCollection
                };

                // ToDo: why we need that?
                //myCollection.PartitionKey.Paths.Add("/RefId");
                myCollection.PartitionKey.Paths.Add("/FlowName");

                myCollection.UniqueKeyPolicy = new UniqueKeyPolicy
                {
                    UniqueKeys =
                    new Collection<UniqueKey>
                    {
                        new UniqueKey { Paths = new Collection<string> { "/RefId" } },
                    }
                };

                var result = _client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(_database.Id), myCollection, new RequestOptions { OfferThroughput = DEFAULT_THROUGHPUT }).Result;
                //CreateIdentityDocument(_client).Wait();
                return result;
            }
        }

        private async Task<DocumentClient> GetClientAndCreateDatabaseIfNeeded(string id)
        {
            var settigns = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _knownTypesBinder
            };

            var client = new DocumentClient(
                new Uri(_databaseUri), _databaseKey, settigns);

            // Get the database by name, or create a new one if one with the name provided doesn't exist.
            // Create a query object for database, filter by name.
            var feed = new FeedOptions { };
            IEnumerable<Database> query = from db in client.CreateDatabaseQuery()
                                          where db.Id == id
                                          select db;

            // Run the query and get the database (there should be only one) or null if the query didn't return anything.
            // Note: this will run synchronously. If async exectution is preferred, use IDocumentServiceQuery<T>.ExecuteNextAsync.
            _database = query.FirstOrDefault();

            if (_database == null)
            {
                // Create the database.
                var options = new RequestOptions { OfferThroughput = DEFAULT_THROUGHPUT };
                _database = await client.CreateDatabaseAsync(new Database { Id = id }, options);
            }

            return client;
        }

        #endregion

        public async Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity)
        {
            try
            {
                flowEntity.TenantId = tenantId ?? flowEntity.TenantId;
                flowEntity.EnvTag = _environmentTag;
                var result = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection), flowEntity);
                return result.Resource.Id;
            }
            catch(Exception exc)
            {
                _logStreamer.TrackException(exc);
                throw;
            }
        }

        public async Task<FlowEntity> GetFlowByRef(string tenantId, string refId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var query = _client
                    .CreateDocumentQuery<FlowEntity>(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection), new FeedOptions { EnableCrossPartitionQuery = true })
                    .Where(f => f.RefId == refId);

                if (!string.IsNullOrEmpty(tenantId))
                {
                    query = query.Where(f => f.TenantId == tenantId);
                }

                using var request = query.Take(1).AsDocumentQuery();

                return (await request.ExecuteNextAsync()).FirstOrDefault();
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"[GetFlowByFlowRunId] Elapsed {elapsedMs} ms");
            }
        }

        public async IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
        {
            var query = _client
                .CreateDocumentQuery<FlowEntity>(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(f => f.EnvTag == _environmentTag &&
                    f.FlowStatus != FlowStatus.Deleted && f.FlowStatus != FlowStatus.Finished &&
                    f.FlowName == flowName);

            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(f => f.TenantId == tenantId);
            }

            using var request = query.
                Select(f => f.RefId)
                .AsDocumentQuery();

            await foreach(var item in request.AsAsyncEnumerable())
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId)
        {
            var query = _client
                .CreateDocumentQuery<FlowEntity>(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection), new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(f => f.EnvTag == _environmentTag && 
                    f.Context.ExecutionResult.IsWaitTask == true && f.FlowStatus != FlowStatus.Deleted 
                    && f.FlowStatus != FlowStatus.Finished);

            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(f => f.TenantId == tenantId);
            }

            using var request = query.
                Select(f => f.RefId)
                .AsDocumentQuery();

            await foreach (var item in request.AsAsyncEnumerable())
            {
                yield return item;
            }
        }

        private struct FlowIdModel
        {
            public string RefId;
            public dynamic Model;
        }

        private IEnumerable<(string RefId, T)> CastoToFlowModel<T>(IEnumerable<FlowIdModel> p) where T : class, IFlowModel
        {
            return p.Select(i =>
            {
                try
                {
                    // TODO: Confirm this is the best performance option
                    //var item = new { PK = 0L, LastModel = default(T) };
                    //var value = CastTo(item, i);
                    //return ((long)value.PK, (T)value.LastModel);
                    var cast = (FlowIdModel)i;
                    return (cast.RefId, (T)cast.Model);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to deserialize flow RefId {i.RefId}, {ex}");
                }
                return (null, null);
            })
                .Where(i => i.Item2 != null);
        }

        public async IAsyncEnumerable<(string, T)> GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
        {
            if (flowModelsQueryOptions.FlowStatuses == null)
            {
                flowModelsQueryOptions.FlowStatuses = new List<FlowStatus> { FlowStatus.Created, FlowStatus.Started, FlowStatus.Waiting, FlowStatus.Failed };
            }

            var q = _client.CreateDocumentQuery<FlowEntity>(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection), new FeedOptions { EnableCrossPartitionQuery = true });
            var query = q
                    .Where(f => f.EnvTag == _environmentTag)
                    .Where(f => f.FlowStatus != FlowStatus.Deleted && flowModelsQueryOptions.FlowStatuses.Contains(f.FlowStatus))
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
                    query = query.Where(f => f.FlowTags.Count(ft => flowModelsQueryOptions.Tags.Contains(ft)) == flowModelsQueryOptions.Tags.Count());
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
            var selector = query.Select(f => new FlowIdModel() { RefId = f.RefId, Model = f.Context.Model });
            using var request = selector.AsDocumentQuery();

            await foreach (var item in request.AsAsyncEnumerable(p => CastoToFlowModel<T>(p)))
            {
                yield return item;
            }
        }

        public async Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            if (flowModelsQueryOptions.FlowStatuses == null)
            {
                flowModelsQueryOptions.FlowStatuses = new List<FlowStatus> { FlowStatus.Created, FlowStatus.Started, FlowStatus.Waiting, FlowStatus.Failed };
            }

            var q = _client.CreateDocumentQuery<FlowEntity>(UriFactory.CreateDocumentCollectionUri(_database.Id, FlowCollection), new FeedOptions { EnableCrossPartitionQuery = true });
            var query = q
                    .Where(f => f.EnvTag == _environmentTag)
                    .Where(f => f.FlowStatus != FlowStatus.Deleted && flowModelsQueryOptions.FlowStatuses.Contains(f.FlowStatus))
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
                    query = query.Where(f => f.FlowTags.Count(ft => flowModelsQueryOptions.Tags.Contains(ft)) == flowModelsQueryOptions.Tags.Count());
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

            //var selector = query.Select(f => f.Context);
            //var data = await selector.ToListAsync();

            var data = query.Select(f => f.Context).AsDocumentQuery();

            var result = new List<FlowContextJsonModel>();
            var jsons = new List<JObject>();

            while (data.HasMoreResults)
            {
                var page = await data.ExecuteNextAsync<JObject>();
                //var page = await data.ExecuteNextAsync<FlowContext>();
                jsons.AddRange(page);
                //result.AddRange(page);
            }

            foreach(var json in jsons)
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
                catch(Exception exc)
                {

                }
               
            }

            return result;
        }

        private async Task<IEnumerable<(string, T)>> FetchQueryResults<T>(IDocumentQuery<FlowIdModel> query) where T : class, IFlowModel
        {
            var result = new List<(string, T)>();
            while (query.HasMoreResults)
            {
                var page = await query.ExecuteNextAsync();

                result.AddRange(
                    page.Select(i =>
                    {
                        try
                        {
                            // TODO: Confirm this is the best performance option
                            //var item = new { PK = 0L, LastModel = default(T) };
                            //var value = CastTo(item, i);
                            //return ((long)value.PK, (T)value.LastModel);
                            var cast = (FlowIdModel)i;
                            return (cast.RefId, (T)cast.Model);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to deserialize flow RefId {i.RefId}, {ex}");
                        }
                        return (null, null);
                    })
                    .Where(i => i.Item2 != null)
                );
            }

            return result;
        }
    }
}
