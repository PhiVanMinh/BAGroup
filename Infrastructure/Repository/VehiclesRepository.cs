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
    /// <summary>Thông tin xe</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class VehiclesRepository : IVehiclesRepository
    {
        private readonly DapperContext _dapperContext;

        public VehiclesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        /// <summary>Lấy thông tin xe theo đơn vị vận tải</summary>
        /// <param name="input">Mã đơn vị vận tải</param>
        /// <returns>Thông tin các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
        public async Task<IEnumerable<Vehicles>> GetAllByCompany(int input)
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [Vehicle.Vehicles] WITH(NOLOCK)
                              WHERE IsDeleted = 0 AND FK_CompanyID = @CompanyID";
                var result = await connection.QueryAsync<Vehicles>(query, new
                {
                    CompanyID = input
                });
                return result;
            }
        }
    }
}
