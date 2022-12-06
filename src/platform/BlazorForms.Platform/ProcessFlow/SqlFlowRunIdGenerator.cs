using Microsoft.Extensions.Configuration;
using BlazorForms.Flows;
using BlazorForms.Platform.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.ProcessFlow
{
    public class SqlFlowRunIdGenerator : IFlowRunIdGenerator
    {
        private readonly string _connectionString;

        public SqlFlowRunIdGenerator(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlFlowRepositoryConnection");
        }

        public async Task<int> GetNextFlowRunId()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ConfigurationException("SqlFlowRepositoryConnection json config not found.");
            }

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT NEXT VALUE FOR [dbo].[flow_id_sequence] AS next;", sql))
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
