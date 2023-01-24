using Newtonsoft.Json;
using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows;
using Npgsql;
using SJ = System.Text.Json;

namespace BlazorForms.Platform
{
    // ToDo: commented out for flowRunId removal refactoring
//    public class NpgsqlFlowRepository : IFlowRepository
//    {
//        private readonly ILogger _logger;
//        private readonly IKnownTypesBinder _knownTypesBinder;
//        private readonly JsonSerializerSettings _jsonSerializerSettings;
//        private readonly IFlowRunIdGenerator _flowRunIdGenerator;
//        private ILogStreamer _logStreamer;
//        private readonly string _connectionString;

//        public NpgsqlFlowRepository(IKnownTypesBinder knownTypesBinder, ILogger<NpgsqlFlowRepository> logger, IFlowRunIdGenerator flowRunIdGenerator,
//            IConfiguration configuration, ILogStreamer logStreamer)
//        {
//            _logger = logger;
//            _logStreamer = logStreamer;
//            _knownTypesBinder = knownTypesBinder;
//            _jsonSerializerSettings = new JsonSerializerSettings
//            {
//                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
//                TypeNameHandling = TypeNameHandling.Objects,
//                SerializationBinder = _knownTypesBinder
//            };

//            _flowRunIdGenerator = flowRunIdGenerator;

//            _connectionString = configuration.GetConnectionString("PostgresNoSqlConnection");
//        }

//        public async Task<T> CloneObject<T>(T source)
//        {
//            Task<T> task = Task.Run(() =>
//            {
//                var json = JsonConvert.SerializeObject(source, _jsonSerializerSettings);
//                var result = JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
//                return result;
//            });

//            return await task;
//        }

//        public async Task<IEnumerable<int>> GetActiveFlows(string flowName)
//        {
//            using (NpgsqlConnection sql = new NpgsqlConnection(_connectionString))
//            {
//                string cmdText = @"SELECT id FROM flow_run WHERE flow_json->>'FlowName' = @p1;";

//                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, sql))
//                {
//                    cmd.CommandType = System.Data.CommandType.Text;
//                    cmd.Parameters.AddWithValue("p1", flowName);
//                    await sql.OpenAsync();
//                    var reader = await cmd.ExecuteReaderAsync();
//                    var list = new CardList<int>();

//                    while (await reader.ReadAsync())
//                    {
//                        list.Add(reader.GetInt32(0));
//                    }

//                    return list;
//                }
//            }
//        }

//        public async Task<FlowEntity> GetFlowByFlowRunId(int flowRunId)
//        {
//            using (NpgsqlConnection sql = new NpgsqlConnection(_connectionString))
//            {
//                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT flow_json FROM flow_run WHERE id=@p1;", sql))
//                {
//                    cmd.CommandType = System.Data.CommandType.Text;
//                    cmd.Parameters.AddWithValue("p1", flowRunId);
//                    await sql.OpenAsync();
//                    var result = await cmd.ExecuteScalarAsync();
//                    var json = Convert.ToString(result);
//                    var data = FlowEntityWrapper.Deserialize(json, _knownTypesBinder);
//                    return data;
//                }
//            }
//        }

//        public async Task<IEnumerable<FlowLastModel>> GetFlowLastModelsByIds(IEnumerable<int> flowRunIdList)
//        {
//            using (NpgsqlConnection sql = new NpgsqlConnection(_connectionString))
//            {
//                var ps = String.Join(",", flowRunIdList);
//                string cmdText = $"SELECT id, flow_json->>'LastModel', flow_json->>'ModelFullName' FROM flow_run WHERE id IN({ps}) ORDER BY id DESC;";

//                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, sql))
//                {
//                    cmd.CommandType = System.Data.CommandType.Text;

//                    var list = new CardList<FlowLastModel>();

//                    if (flowRunIdList.Count() > 0)
//                    {
//                        await sql.OpenAsync();
//                        var reader = await cmd.ExecuteReaderAsync();

//                        while (await reader.ReadAsync())
//                        {
//                            var targetType = _knownTypesBinder.KnownTypesDict[reader.GetString(2)];
//                            var val = new FlowLastModel
//                            {
//                                FlowRunId = reader.GetInt32(0),
//                                LastModel = SJ.JsonSerializer.Deserialize(reader.GetString(1), targetType) as IFlowModel
//                            };

//                            list.Add(val);
//                        }
//                    }
//                    return list;
//                }
//            }
//        }

//        public async Task<int> GetNextIdentity()
//        {
//            return await _flowRunIdGenerator.GetNextFlowRunId();
//        }

//        public async Task<string> UpsertFlow(FlowEntity flowEntity)
//        {
//            using (NpgsqlConnection sql = new NpgsqlConnection(_connectionString))
//            {
//                string cmdText = @"
//        INSERT INTO flow_run AS d (id, flow_json) VALUES (@p1, @p2)
//        ON CONFLICT (id) DO UPDATE
//        SET flow_json = @p2
//        WHERE d.id = @p1;
//";
//                using (NpgsqlCommand cmd = new NpgsqlCommand(cmdText, sql))
//                {
//                    var dto = FlowEntityWrapper.CreateInstance(flowEntity);
//                    var json = SJ.JsonSerializer.Serialize(dto, typeof(object), new SJ.JsonSerializerOptions { IgnoreReadOnlyProperties = true });

//                    cmd.CommandType = System.Data.CommandType.Text;
//                    cmd.Parameters.AddWithValue("p1", flowEntity.PK);
//                    cmd.Parameters.Add(new NpgsqlParameter("p2", NpgsqlTypes.NpgsqlDbType.Jsonb) { Value = json });
//                    await sql.OpenAsync();
//                    var result = await cmd.ExecuteNonQueryAsync();
//                    return flowEntity.PK.ToString();
//                }
//            }
//        }

//        Task<IEnumerable<(int, T)>> IFlowRepository.GetFlowsLastModels<T>(string flowName, string refIf = null)
//        {
//            throw new NotImplementedException();
//        }

//        Task<IEnumerable<(int, T)>> IFlowRepository.GetFlowsLastModels<T>(IEnumerable<int> flowIds, string refId = null)
//        {
//            throw new NotImplementedException();
//        }

//        Task<IEnumerable<(int, T)>> IFlowRepository.GetFlowsLastModels<T>(string flowName, IEnumerable<string> tags)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<FlowEntity> GetFlowByRefId(string refId)
//        {
//            throw new NotImplementedException();
//        }
//    }
}
