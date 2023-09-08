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
    public class SpeedOversRepository : ISpeedOversRepository
    {
        private readonly DapperContext _dapperContext;

        public SpeedOversRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        //public Task<IEnumerable<SpeedOvers>> GetAll(SpeedViolationVehicleInput input)
        //{
        //    using (var connection = _dapperContext.CreateConnection("ServerLab3"))
        //    {
        //        var query = @"SELECT * FROM [BGT.SpeedOvers] WITH(NOLOCK)";
        //        var result = connection.QueryAsync<SpeedOvers>(query);
        //        return result;
        //    }
        //}

        public async Task<IEnumerable<SpeedOvers>> GetAllByDate(SpeedViolationVehicleInput input)
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [BGT.SpeedOvers] WITH(NOLOCK)
                              WHERE StartTime BETWEEN @FromDate AND @ToDate";
                var result = await connection.QueryAsync<SpeedOvers>(query, new
                {
                    input.FromDate,
                    input.ToDate,
                });
                return result;
            }
        }
    }
}
