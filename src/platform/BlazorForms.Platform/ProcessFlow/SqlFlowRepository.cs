using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Platform
{
    public class SqlFlowRepository : IFlowRepository
    {
        private readonly ILogger _logger;
        private readonly IKnownTypesBinder _knownTypesBinder;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly string _connectionString;
        private static readonly string _environmentTag = Environment.MachineName;

        public SqlFlowRepository(IKnownTypesBinder knownTypesBinder, ILogger<SqlFlowRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _knownTypesBinder = knownTypesBinder;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = _knownTypesBinder
            };

            _connectionString = configuration.GetConnectionString("SqlFlowRepositoryConnection");
        }

        public async Task<string> UpsertFlow(string tenantId, FlowEntity flowEntity)
        {
            flowEntity.EnvTag = _environmentTag;
            flowEntity.TenantId = tenantId;

            using SqlConnection sql = new(_connectionString);
            string cmdText = @"
IF EXISTS(SELECT 1 FROM [dbo].[flow_run] WHERE id = @p1)
    UPDATE [dbo].[flow_run] SET flow_json = @p2 WHERE id = @p1;
ELSE
    INSERT INTO [dbo].[flow_run]
        SELECT @p1, @p2;
";

            using SqlCommand cmd = new(cmdText, sql);
            var json = JsonConvert.SerializeObject(flowEntity, _jsonSerializerSettings);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddWithValue("p1", flowEntity.RefId);
            cmd.Parameters.AddWithValue("p2", json);
            await sql.OpenAsync();
            var result = await cmd.ExecuteNonQueryAsync();
            return flowEntity.RefId;
        }

        public async Task<FlowEntity> GetFlowByRef(string tenantId, string refId)
        {
            List<FlowStatus> flowStatuses = new List<FlowStatus> { FlowStatus.Created, FlowStatus.Started, FlowStatus.Waiting, FlowStatus.Failed, FlowStatus.Finished };
            var flow = await GetFlows(tenantId, new FlowModelsQueryOptions { RefIds = new List<string> { refId }, FlowStatuses = flowStatuses }, true).FirstOrDefaultAsync();

            return JsonConvert.DeserializeObject<FlowEntity>(flow.Item2.ToString(), _jsonSerializerSettings);
        }

        public async IAsyncEnumerable<string> GetActiveFlowsIds(string tenantId, string flowName)
        {
            FlowModelsQueryOptions queryOptions = new FlowModelsQueryOptions { FlowName = flowName };

            await foreach (var item in GetFlows(flowModelsQueryOptions: queryOptions))
            {
                yield return item.Item1;
            }
        }

        public async Task<IFlowModel> GetFlowLastModel(string refId)
        {
            var flow = await GetFlows(flowModelsQueryOptions : new FlowModelsQueryOptions { RefIds = new List<string> { refId } }).FirstOrDefaultAsync();
            var jsonModel = flow.Item2.SelectToken("$.Context.Model");

            var result = JsonConvert.DeserializeObject(jsonModel.ToString(), _jsonSerializerSettings);
            return result as IFlowModel;
        }

        public async IAsyncEnumerable<(string, T)> GetFlowsLastModels<T>(IEnumerable<string> refIds) where T : class, IFlowModel
        {
            FlowModelsQueryOptions queryOptions = new FlowModelsQueryOptions { RefIds = refIds };

            await foreach (var item in GetFlows(flowModelsQueryOptions: queryOptions))
            {
                var jsonModel = item.Item2.SelectToken("$.Context.Model");
                var value = (item.Item1, JsonConvert.DeserializeObject<T>(jsonModel.ToString(), _jsonSerializerSettings));
                yield return value;
            }
        }

        public async IAsyncEnumerable<(string, T)> GetFlowModels<T>(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions) where T : class, IFlowModel
        {
            await foreach (var item in GetFlows(tenantId : tenantId, flowModelsQueryOptions: flowModelsQueryOptions))
            {
                var jsonModel = item.Item2.SelectToken("$.Context.Model");
                var value = (item.Item1, JsonConvert.DeserializeObject<T>(jsonModel.ToString(), _jsonSerializerSettings));
                yield return value;
            }
        }

        public async IAsyncEnumerable<(string, T)> GetFlowsLastModels<T>(string flowName, string refId = null) where T : class, IFlowModel
        {
            FlowModelsQueryOptions queryOptions = new FlowModelsQueryOptions { FlowName = flowName };

            if (refId != null)
            {
                queryOptions.RefIds = new List<string> { refId };
            }

            await foreach (var item in GetFlows(flowModelsQueryOptions : queryOptions))
            {
                var jsonModel = item.Item2.SelectToken("$.Context.Model");
                var value = (item.Item1, JsonConvert.DeserializeObject<T>(jsonModel.ToString(), _jsonSerializerSettings));
                yield return value;
            }
        }

        public async IAsyncEnumerable<string> GetAllWaitingFlowsIds(string tenantId)
        {
            await foreach (var item in GetFlows(tenantId : tenantId, new FlowModelsQueryOptions { FlowStatuses = new List<FlowStatus> { FlowStatus.Waiting } }, checkEnvTag : true))
            {
                yield return item.Item1;
            }
        }

        public async Task<List<FlowContextJsonModel>> GetFlowContexts(string tenantId, FlowModelsQueryOptions flowModelsQueryOptions)
        {
            List<FlowContextJsonModel> result = new List<FlowContextJsonModel>();

            //ToDo: Apply Filters and pagination

            await foreach (var item in GetFlows(tenantId, flowModelsQueryOptions, checkEnvTag: true))
            {
                try
                {
                    var json = item.Item2;
                    var jsonCtx = item.Item2.SelectToken("$.Context");
                    var modelTypeName = json.SelectToken("$.Context.Model.$type").ToString();

                    FlowContextJsonModel context;

                    if (_knownTypesBinder.KnownTypesDict.ContainsKey(modelTypeName))
                    {
                        var ctx = JsonConvert.DeserializeObject<FlowContext>(jsonCtx.ToString(), _jsonSerializerSettings);
                        context = new FlowContextJsonModel(ctx, ctx.Model);
                    }
                    else
                    {
                        // Model cannot be deserialized, use null model
                        var contextNoModel = JsonConvert.DeserializeObject<FlowContextNoModel>(jsonCtx.ToString());
                        context = new FlowContextJsonModel(contextNoModel, null);
                    }

                    var jsonModel = json.SelectToken("$.Context.Model");
                    context.ModelJson = jsonModel.ToString();
                    context.ModelType = modelTypeName;
                    result.Add(context);
                }
                catch (Exception exc)
                {

                }
            };            

            return result;
        }

        private async IAsyncEnumerable<(string, JObject)> GetFlows(string tenantId = "", FlowModelsQueryOptions flowModelsQueryOptions = null, bool checkEnvTag = false)
        {                        
            if (flowModelsQueryOptions == null)
            {
                flowModelsQueryOptions = new FlowModelsQueryOptions();
            }
            
            using SqlConnection sql = new(_connectionString);
            
            string cmdText = $"select id, flow_json from flow_run";
            string cmdCondition = string.Empty;

            if (checkEnvTag)
            {
                cmdCondition = $" where JSON_VALUE(flow_json, '$.EnvTag') = '{_environmentTag}'";
            } 

            if (!string.IsNullOrEmpty(tenantId))
            {
                cmdCondition = string.IsNullOrEmpty(cmdCondition)
                    ? $" where JSON_VALUE(flow_json, '$.TenantId') = '{tenantId}'"
                    : cmdCondition + $"and JSON_VALUE(flow_json, '$.TenantId') = '{tenantId}'";
            }

            if (!string.IsNullOrEmpty(flowModelsQueryOptions.FlowName))
            {
                cmdCondition = string.IsNullOrEmpty(cmdCondition)
                    ? $" where JSON_VALUE(flow_json, '$.FlowName') = '{flowModelsQueryOptions.FlowName}'"
                    : cmdCondition + $"and JSON_VALUE(flow_json, '$.FlowName') = '{flowModelsQueryOptions.FlowName}'";
            }

            if (flowModelsQueryOptions.FlowStatuses == null)
            {
                flowModelsQueryOptions.FlowStatuses = new List<FlowStatus> { FlowStatus.Created, FlowStatus.Started, FlowStatus.Waiting, FlowStatus.Failed };

                int i = 0;
                var parameters = flowModelsQueryOptions.FlowStatuses.Select(r => $"@flowStatus{i++}");
                var flowStatuses = string.Join(",", parameters);

                cmdCondition = string.IsNullOrEmpty(cmdCondition)
                    ? $" where JSON_VALUE(flow_json, '$.FlowName') in ({flowStatuses})'"
                    : cmdCondition + $" and JSON_VALUE(flow_json, '$.FlowStatus') in ({flowStatuses})";
            }

            if (flowModelsQueryOptions.RefIds != null && flowModelsQueryOptions.RefIds.Any())
            {
                int i = 0;                
                var refIds = string.Join(",", flowModelsQueryOptions.RefIds.Select(f => $"'{f}'").ToList());

                cmdCondition = string.IsNullOrEmpty(cmdCondition)
                    ? $" where id in ({refIds})'"
                    : cmdCondition + $" and id in ({refIds})";
            }                                                

            cmdText += cmdCondition;
            cmdText += " order by JSON_VALUE(flow_json, '$.Created') desc"; //Default sorting

            using SqlCommand cmd = new(cmdText, sql);
            cmd.CommandType = System.Data.CommandType.Text;

            for (int j = 0; j < flowModelsQueryOptions.FlowStatuses.Count(); j++)
            {
                cmd.Parameters.AddWithValue($"flowStatus{j}", flowModelsQueryOptions.FlowStatuses.ToList()[j]);
            }

            // ToDo: RefIds condition already added a few statements above
            //if (flowModelsQueryOptions.RefIds != null && flowModelsQueryOptions.RefIds.Any())
            //{
            //    for (int j = 0; j < flowModelsQueryOptions.RefIds.Count(); j++)
            //    {
            //        cmd.Parameters.AddWithValue($"refId{j}", flowModelsQueryOptions.RefIds.ToList()[j]);
            //    }
            //}

            await sql.OpenAsync();

            List<(string, JObject)> flows = new List<(string, JObject)>();

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return (reader.GetString(0), JObject.Parse(reader.GetString(1)));
            }
        }

        private IQueryable<FlowEntity> ApplyQueryFilters(IQueryable<FlowEntity> query, FlowModelsQueryOptions flowModelsQueryOptions)
        {
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

            return query;
        }
    }
}
