using Application.Dto.SpeedViolation;
using Application.Interfaces;
using Dapper;
using Infrastructure.Contexts;

namespace Infrastructure.Repository
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/6/2023   created
    /// </Modified>
    public class SpeedViolationRepository : ISpeedViolationRepository
    {
        private readonly DapperContext _dapperContext;

        public SpeedViolationRepository( DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        /// <summary>Lấy danh sách các xe theo công ty</summary>
        /// <param name="input">Mã công ty</param>
        /// <returns>
        ///   Danh sách các xe của công ty
        /// </returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/6/2023   created
        /// </Modified>
        public async Task<List<GetVehicleListDto>> GetVehicleByCompanyId(int input)
		{
            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
				var query = @"SELECT vhc.PK_VehicleID VehicleID, vhc.PrivateCode
							FROM [Vehicle.Vehicles] vhc WITH(NOLOCK)
							WHERE vhc.FK_CompanyID = @CompanyId AND IsDeleted = 0";
                var result = await connection.QueryAsync<GetVehicleListDto>(query, new
                {
                    CompanyId = input
                });
                return result.ToList();
            }
        }
    }
}
