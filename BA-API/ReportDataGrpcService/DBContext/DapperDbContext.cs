using System.Data.SqlClient;
using System.Data;

namespace ReportDataGrpcService.DBContext
{
    public class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        public DapperDbContext(IConfiguration configuration)
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
