using BlazorForms.Flows;
using BlazorForms.Platform.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

/*
namespace BlazorForms.Platform
{
    public class NpgsqlFlowRunIdGenerator : IFlowRunIdGenerator
    {
        private readonly string _connectionString;

        public NpgsqlFlowRunIdGenerator(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostgresNoSqlConnection");
        }

        public async Task<int> GetNextFlowRunId()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ConfigurationException("PostgresNoSqlConnection json config not found.");
            }

            using (NpgsqlConnection sql = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT nextval('flow_id_sequence');", sql))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    await sql.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }
    }
}
*/