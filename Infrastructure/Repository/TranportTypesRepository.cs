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
    /// <summary>Thông tin loại hình vận tải</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/11/2023   created
    /// </Modified>
    public class TranportTypesRepository : ITranportTypesRepository
    {
        private readonly DapperContext _dapperContext;

        public TranportTypesRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        /// <summary>Lấy các thông tin loại hình vận tải</summary>
        /// <returns> Các thông tin loại hình vận tải</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/11/2023   created
        /// </Modified>
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
