using Application.Dto.SpeedViolation;
using Application.Interfaces;
using Application.Model;
using Dapper;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <summary>Thông tin vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class SpeedOversRepository : ISpeedOversRepository
    {
        private readonly DapperContext _dapperContext;

        public SpeedOversRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        /// <summary>Thông tin vi phạm tốc độ theo ngày</summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Các thông tin vi phạm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<SpeedOvers>> GetAllByDate(DateTime? fromDate, DateTime? toDate)
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [BGT.SpeedOvers] WITH(NOLOCK)
                              WHERE StartTime BETWEEN @FromDate AND @ToDate";
                var result = await connection.QueryAsync<SpeedOvers>(query, new
                {
                   FromDate = fromDate,
                   ToDate = toDate,
                });
                return result;
            }
        }
    }
}
