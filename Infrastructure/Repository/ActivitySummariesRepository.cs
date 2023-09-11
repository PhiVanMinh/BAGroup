using Application.Dto.SpeedViolation;
using Application.Interfaces;
using Application.Model;
using Dapper;
using Infrastructure.Contexts;


namespace Infrastructure.Repository
{
    /// <summary>Lây thông tin tổng hợp</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class ActivitySummariesRepository : IActivitySummariesRepository 
    {
        private readonly DapperContext _dapperContext;

        public ActivitySummariesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        /// <summary>Lấy thông tin tổng hợp theo đơn vị vẫn tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin tổng hợp</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<ActivitySummaries>> GetAllByCompany(int input)
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [Report.ActivitySummaries] WITH(NOLOCK)
                              WHERE FK_CompanyID = @CompanyID";
                var result = await connection.QueryAsync<ActivitySummaries>(query, new
                {
                    CompanyID = input
                });
                return result;
            }
        }
    }
}
