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
    public class VehicleTransportTypesRepository : IVehicleTransportTypesRepository
    {
        private readonly DapperContext _dapperContext;

        public VehicleTransportTypesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<VehicleTransportTypes>> GetAll()
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [BGT.VehicleTransportTypes] WITH(NOLOCK)";
                var result = await connection.QueryAsync<VehicleTransportTypes>(query);
                return result;
            }
        }
    }
}
