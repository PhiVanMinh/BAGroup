using Application.Dto.SpeedViolation;
using Application.Interfaces;
using Application.Model;
using Dapper;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class VehiclesRepository : IVehiclesRepository
    {
        private readonly DapperContext _dapperContext;

        public VehiclesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<IEnumerable<Vehicles>> GetAllByCompany(SpeedViolationVehicleInput input)
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [Vehicle.Vehicles] WITH(NOLOCK)
                              WHERE IsDeleted = 0 AND FK_CompanyID = @CompanyID";
                var result = await connection.QueryAsync<Vehicles>(query, new
                {
                    CompanyID = input.CompanyId
                });
                return result;
            }
        }
    }
}
