using Application.Dto.SpeedViolation;
using Application.Interfaces;
using Application.Model;
using Dapper;
using Infrastructure.Contexts;


namespace Infrastructure.Repository
{
    public class ActivitySummariesRepository : IActivitySummariesRepository 
    {
        private readonly DapperContext _dapperContext;

        public ActivitySummariesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<IEnumerable<ActivitySummaries>> GetAll(SpeedViolationVehicleInput input)
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [Report.ActivitySummaries] WITH(NOLOCK)
                              WHERE FK_CompanyID = @CompanyID";
                var result = await connection.QueryAsync<ActivitySummaries>(query, new
                {
                    CompanyID = input.CompanyId
                });
                return result;
            }
        }
    }
}
