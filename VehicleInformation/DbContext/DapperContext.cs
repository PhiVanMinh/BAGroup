using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace VehicleInformation.DbContext
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDbConnection CreateConnection(string input)
        {
            var connectionString = _configuration.GetConnectionString(input);
            return new SqlConnection(connectionString);
        }
    }
}
