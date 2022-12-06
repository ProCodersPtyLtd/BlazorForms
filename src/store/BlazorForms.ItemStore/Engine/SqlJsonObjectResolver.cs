using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Shared;

namespace BlazorForms.ItemStore
{
    public class SqlJsonObjectResolver : IStoreObjectResolver
    {
        private readonly StoreSchema _schema;
        private readonly StoreQueryDefinition _query;
        private readonly QueryParameter[] _parameters;
        private ILogStreamer _logStreamer;

        public SqlJsonObjectResolver(StoreSchema schema, StoreQueryDefinition query, QueryParameter[] parameters, ILogStreamer logStreamer)
        {
            _schema = schema;
            _query = query;
            _parameters = parameters;
            _logStreamer = logStreamer;
        }

        public string ResolveField(StoreFieldReference field)
        {
            string sql;

            if (_query.Tables != null && _query.Tables.ContainsKey(field.ObjectAlias))
            {
                sql = $"JSON_VALUE({field.ObjectAlias}.{SqlScriptHelper.DATA_COLUMN}, '$.{field.FieldName}')";
            }
            else if (_query.SubQueries != null && _query.SubQueries.ContainsKey(field.ObjectAlias))
            {
                sql = $"{ field.ObjectAlias}.{field.FieldName}";
            }
            else
            {
                _logStreamer.TrackException(new SqlJsonQueryBuildException($"Query {_query.QueryAlias} has Object Alias {field.ObjectAlias}, that is not found in tables and sub queries"));
                throw new SqlJsonQueryBuildException($"Query {_query.QueryAlias} has Object Alias {field.ObjectAlias}, that is not found in tables and sub queries");
            }

            return sql;
        }
    }
}
