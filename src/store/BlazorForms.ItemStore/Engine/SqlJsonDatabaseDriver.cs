using Microsoft.Extensions.Configuration;
using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using BlazorForms.Shared;

namespace BlazorForms.ItemStore
{
    public class SqlJsonDatabaseDriver : IStoreDatabaseDriver
    {
        private const string MAIN_SEQ = "main_seq";

        private readonly IConfiguration _config;
        private readonly string _connectionString;
        private ILogStreamer _logStreamer;

        public SqlJsonDatabaseDriver(ILogStreamer logStreamer)
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = _config.GetSection("SqlJsonDatabaseDriver:ConnectionString")?.Value;
            _logStreamer = logStreamer;
        }

        // Schema
        public async Task ValidateSchema(StoreSchema storeSchema)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDatabaseObjects(StoreSchema storeSchema)
        {
            var sql = SqlScriptHelper.CreateSchema(storeSchema.Name);
            sql += SqlScriptHelper.CreateSequence(MAIN_SEQ, storeSchema.Name);

            foreach(var table in storeSchema.Definitions.Values)
            {
                sql += SqlScriptHelper.CreateJsonTable(table.Name, storeSchema.Name);
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ValidateDatabaseObjects(StoreSchema storeSchema)
        {
            throw new NotImplementedException();
        }

        // Query
        public async Task DeleteItem(StoreSchema storeSchema, StoreObject item)
        {
            throw new NotImplementedException();
        }

        public async Task<StoreRecordset> ExecuteQuery(StoreSchema storeSchema, StoreQuery storeQuery, params QueryParameter[] parameters)
        {
            var result = new StoreRecordset { Model = GetQueryModel(storeSchema, storeQuery) };
            var sql = GenerateQuery(storeSchema,  storeQuery, parameters);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // set parameters
                    foreach(var p in parameters)
                    {
                        cmd.Parameters.AddWithValue(p.Name, p.Value);
                    }

                    await conn.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    var resDict = new Dictionary<string, List<object>>();

                    if(storeQuery.Query.Fields == null || storeQuery.Query.Fields.Count() == 0)
                    {
                        _logStreamer.TrackException(new SqlJsonQueryBuildException($"Query {storeQuery.Query.QueryAlias} doesn't have output fields"));
                        throw new SqlJsonQueryBuildException($"Query {storeQuery.Query.QueryAlias} doesn't have output fields");
                    }

                    foreach (var field in storeQuery.Query.Fields.Values)
                    {
                        resDict[field.FieldAlias] = new List<object>();
                    }

                    while (await reader.ReadAsync())
                    {
                        foreach(var field in storeQuery.Query.Fields.Values)
                        {
                            var val = reader[field.FieldAlias];
                            resDict[field.FieldAlias].Add(val);
                        }
                    }

                    result.Records = new Dictionary<string, object[]>();

                    foreach (var field in storeQuery.Query.Fields.Values)
                    {
                        result.Records[field.FieldAlias] = resDict[field.FieldAlias].ToArray();
                    }
                }
            }

            return result;
        }

        private ModelObject GetQueryModel(StoreSchema storeSchema, StoreQuery storeQuery)
        {
            var result = new ModelObject { Name = storeQuery.Name, Fields = new Dictionary<string, ModelField>() };

            foreach (var field in storeQuery.Query.Fields.Values)
            {
                string type = "";

                // resolve type from Table
                if(storeQuery.Query.Tables.ContainsKey(field.Field.ObjectAlias))
                {
                    var tableName = storeQuery.Query.Tables[field.Field.ObjectAlias].TableName;
                    var table = storeSchema.Definitions[tableName];
                    var tableField = table.Properties[field.Field.FieldName];
                    type = tableField.Type;
                }
                else
                {
                    // ToDo: resolve type from Sub Queries
                }

                result.Fields[field.FieldAlias] = new ModelField { Name = field.FieldAlias, Type = type };
            }
                    
            return result;
        }

        private string GenerateQuery(StoreSchema storeSchema, StoreQuery storeQuery, QueryParameter[] parameters)
        {
            var resolver = new SqlJsonObjectResolver(storeSchema, storeQuery.Query, parameters, _logStreamer);
            var expressionBuilder = new ExpressionBuilder(resolver);
            var sql = GenerateSubQuery(expressionBuilder, storeSchema, storeQuery.Query, parameters);
            return sql;
        }

        // ToDo: refactor to use SqlJsonObjectResolver
        private void AppendFromClause(ExpressionBuilder expressionBuilder, StoreSchema schema, StoreQueryDefinition query, QueryParameter[] parameters, StringBuilder sql, string alias)
        {
            if (query.Tables != null && query.Tables.ContainsKey(alias))
            {
                var table = query.Tables[alias];
                sql.AppendLine($"FROM {schema.Name}.{table.TableName} {table.ObjectAlias}");
            }
            else if (query.SubQueries != null && query.SubQueries.ContainsKey(alias))
            {
                var subQuery = query.SubQueries[alias];
                var querySql = GenerateSubQuery(expressionBuilder, schema, subQuery, parameters);
                sql.AppendLine($"FROM ({querySql}) {subQuery.QueryAlias}");
            }
            else
            {
                _logStreamer.TrackException(new SqlJsonQueryBuildException($"Query {query.QueryAlias} has Object Alias {alias}, that is not found in tables and sub queries"));
                throw new SqlJsonQueryBuildException($"Query {query.QueryAlias} has Object Alias {alias}, that is not found in tables and sub queries");
            }
        }

        private string GenerateSubQuery(ExpressionBuilder expressionBuilder, StoreSchema schema, StoreQueryDefinition query, QueryParameter[] parameters)
        {
            var sql = new StringBuilder();

            // Select
            sql.Append("SELECT");
            string comma = "";

            if (query.Fields == null || query.Fields.Count() == 0)
            {
                _logStreamer.TrackException(new SqlJsonQueryBuildException($"Query {query.QueryAlias} doesn't have output fields"));
                throw new SqlJsonQueryBuildException($"Query {query.QueryAlias} doesn't have output fields");
            }

            // ToDo: refactor to use SqlJsonObjectResolver
            foreach (var field in query.Fields.Values)
            {
                if(field.Operation != null || field.RawExpression != null)
                {
                    throw new NotImplementedException("Operations and expressions are not implemented");
                }

                sql.Append($"{comma} JSON_VALUE({field.Field.ObjectAlias}.{SqlScriptHelper.DATA_COLUMN},'$.{field.Field.FieldName}') AS {field.FieldAlias}");
                comma = ",";
            }

            sql.AppendLine();

            // From
            if (query.Joins == null || query.Joins.Count() == 0)
            {
                //if ((query.Tables == null || query.Tables.Count == 0) && (query.SubQueries == null || query.SubQueries.Count == 0))
                //{
                //    throw new SqlJsonQueryBuildException($"Query {query.QueryAlias} doesn't have joins, tables and sub queries");
                //}
                    
                if (query.Tables != null && query.Tables.Count > 0)
                {
                    var table = query.Tables.Single().Value;
                    AppendFromClause(expressionBuilder, schema, query, parameters, sql, table.ObjectAlias);
                    //sql.AppendLine($"FROM {schema.Name}.{table.TableName} {table.ObjectAlias}");
                }
                else if (query.SubQueries != null && query.SubQueries.Count > 0)
                {
                    var subQuery = query.SubQueries.Single().Value;
                    AppendFromClause(expressionBuilder, schema, query, parameters, sql, subQuery.QueryAlias);
                    //var querySql = GenerateSubQuery(schema, subQuery, parameters);
                    //sql.AppendLine($"FROM ({querySql}) {subQuery.QueryAlias}");
                }
                else
                {
                    _logStreamer.TrackException(new SqlJsonQueryBuildException($"Query {query.QueryAlias} doesn't have joins, tables and sub queries"));
                    throw new SqlJsonQueryBuildException($"Query {query.QueryAlias} doesn't have joins, tables and sub queries");
                }
            }
            else
            {
                AppendFromClause(expressionBuilder, schema, query, parameters, sql, query.Joins.First().LeftObjectAlias);

                foreach (var join in query.Joins)
                {
                    string joinType = join.JoinType ?? "";

                    switch(joinType.ToLower())
                    {
                        case "left":
                            joinType = "LEFT OUTER JOIN";
                            break;
                        case "right":
                            joinType = "RIGHT OUTER JOIN";
                            break;
                        case "full":
                            joinType = "FULL OUTER JOIN";
                            break;
                        default:
                            joinType = "INNER JOIN";
                            break;
                    }

                    sql.Append($"{joinType}");

                    if (query.Tables != null && query.Tables.ContainsKey(join.RightObjectAlias))
                    {
                        var table = query.Tables[join.RightObjectAlias];
                        sql.Append($" {schema.Name}.{table.TableName} {table.ObjectAlias}");
                    }
                    else if (query.SubQueries != null && query.SubQueries.ContainsKey(join.RightObjectAlias))
                    {
                        var subQuery = query.SubQueries[join.RightObjectAlias];
                        var querySql = GenerateSubQuery(expressionBuilder, schema, subQuery, parameters);
                        sql.AppendLine();
                        sql.Append($"({querySql}) {subQuery.QueryAlias}");
                    }
                    else
                    {
                        _logStreamer.TrackException(new SqlJsonQueryBuildException($"Query {query.QueryAlias} has RightObjectAlias {join.RightObjectAlias}, that is not found in tables and sub queries"));
                        throw new SqlJsonQueryBuildException($"Query {query.QueryAlias} has RightObjectAlias {join.RightObjectAlias}, that is not found in tables and sub queries");
                    }

                    if(join.LeftComplexFieldAliases != null || join.RightComplexFieldAliases != null)
                    {
                        _logStreamer.TrackException(new NotImplementedException("Joins with complex conditions not implemented"));
                        throw new NotImplementedException("Joins with complex conditions not implemented");
                    }

                    sql.Append(" ON ");
                    sql.Append($"JSON_VALUE({join.LeftObjectAlias}.{SqlScriptHelper.DATA_COLUMN},'$.{join.LeftFieldAlias}') = ");
                    sql.Append($"JSON_VALUE({join.RightObjectAlias}.{SqlScriptHelper.DATA_COLUMN},'$.{join.RightFieldAlias}')");
                    //sql.AppendLine($" ON {join.LeftObjectAlias}.{join.LeftFieldAlias} = {join.RightObjectAlias}.{join.RightFieldAlias}");
            
                    sql.AppendLine();
                }
            }


            // Where
            if(query.Where != null && query.Where.Expression != null)
            {
                var expSql = expressionBuilder.Build(query.Where.Expression);
                sql.AppendLine($"WHERE {expSql}");
            }

            // Group By
            if(query.GroupByFields != null)
            {
                _logStreamer.TrackException(new NotImplementedException("Group By is not implemented"));
                throw new NotImplementedException("Group By is not implemented");
            }

            // Having
            if (query.Having != null)
            {
                _logStreamer.TrackException(new NotImplementedException("Having is not implemented"));
                throw new NotImplementedException("Having is not implemented");
            }

            return sql.ToString();
        }

        public async Task<long> UpsertItem(StoreSchema storeSchema, StoreObject item, long pkid = 0)
        {
            var table = storeSchema.Definitions[item.TableName];
            var pkField = table.Properties.Values.Single(p => p.Pk == true);
            long id = 0;

            if (item.Values.ContainsKey(pkField.Name))
            {
                id = Convert.ToInt64(item.Values[pkField.Name]);
            }

            string sql;
            string p1 = "p1";

            if(id == 0)
            {
                if (pkField.AutoIncrement)
                {
                    sql = SqlScriptHelper.InsertJsonTableAutoIncrement(item.TableName, storeSchema.Name, MAIN_SEQ, pkField.Name, p1);
                }
                else
                {
                    sql = SqlScriptHelper.InsertJsonTable(item.TableName, storeSchema.Name, pkid, pkField.Name, p1);
                }
            }
            else
            {
                sql = SqlScriptHelper.UpdateJsonTable(item.TableName, storeSchema.Name, id, p1);
            }

            string json = JsonConvert.SerializeObject(item.Values);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    cmd.Parameters.AddWithValue(p1, json);
                    var result = await cmd.ExecuteScalarAsync();

                    if(id == 0)
                    {
                        id = Convert.ToInt64(result);
                        item.Values[pkField.Name] = id;
                    }
                }
            }

            return id;
        }


        public void ValidateQuery(StoreSchema storeSchema, StoreQuery storeQuery)
        {
            throw new NotImplementedException();
        }

        public async Task<StoreObject> GetItemById(StoreSchema storeSchema, string tableName, long id)
        {
            var result = new StoreObject { TableName = tableName };
            var table = storeSchema.Definitions[tableName];
            var sql = SqlScriptHelper.SelectByIdJsonTable(tableName, storeSchema.Name, id);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();

                    if (table.Properties == null || table.Properties.Count() == 0)
                    {
                        _logStreamer.TrackException(new SqlJsonQueryBuildException($"Table {table.Name} doesn't have fields"));
                        throw new SqlJsonQueryBuildException($"Table {table.Name} doesn't have fields");
                    }

                    if (await reader.ReadAsync())
                    {
                        // read item
                        var json = reader[SqlScriptHelper.DATA_COLUMN].AsString();
                        result.Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    }
                    else
                    {
                        // populate empty values
                        result.Values = new Dictionary<string, object>();

                        foreach (var p in table.Properties.Values)
                        {
                            result.Values[p.Name] = null;
                        }
                    }
                }
            }

            return result;
        }
    }
}
