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
    public class TranportTypesRepository : ITranportTypesRepository
    {
        private readonly DapperContext _dapperContext;

        public TranportTypesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<TranportTypes>> GetAll()
        {
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var query = @"SELECT * FROM [BGT.TranportTypes] WITH(NOLOCK)";
                var result = await connection.QueryAsync<TranportTypes>(query);
                return result;
            }
        }
    }
}
