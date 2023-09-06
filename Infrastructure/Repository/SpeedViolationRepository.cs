using Application.Dto.SpeedViolation;
using Application.Interfaces;
using Dapper;
using Infrastructure.Contexts;
using System.ComponentModel.Design;

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

        /// <summary>Lấy danh sách các xe vi phạm tốc độ</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Danh sách xe vi phạm theo điều kiện lọc</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/6/2023   created
        /// </Modified>
        public async Task<List<GetAllSpeedViolationVehicleDto>> GetAllSpeedViolationVehicle(SpeedViolationVehicleInput input)
        {
            var query = @"
                        SELECT 
							vhcInfo.PK_VehicleID VehicleID,
							vhcInfo.PrivateCode,
							vhcInfo.DisplayName TransportType,
							spo.SpeedVioLevel1,
							spo.SpeedVioLevel2,
							spo.SpeedVioLevel3,
							spo.SpeedVioLevel4,
							spo.TotalSpeedVio,
							CASE WHEN (atvs.TotalKm is not null AND atvs.TotalKm > 1000) THEN spo.TotalSpeedVio * 1000/ atvs.TotalKm ELSE spo.TotalSpeedVio END RatioSpeedVio,
							spo.TotalKmVio,
							atvs.TotalKm,
							--TotalKmVio * 100/ NULLIF(atvs.TotalKm, 0) as RatioKmVio,
							spo.TotalTimeVio,
							atvs.TotalTime,
							--TotalTimeVio * 100/ NULLIF(atvs.TotalTime, 0)  as RatioTimeVio,
							vhcInfo.VehiclePlate
						FROM 
							(
							SELECT
								SUM(CASE WHEN VelocityAllow + 5 <= VelocityGps AND VelocityGps < VelocityAllow + 10 THEN 1 ELSE 0 END) SpeedVioLevel1,
								SUM(CASE WHEN VelocityAllow + 10 <= VelocityGps AND VelocityGps < VelocityAllow + 20 THEN 1 ELSE 0 END) SpeedVioLevel2,
								SUM(CASE WHEN VelocityAllow + 20 <= VelocityGps AND VelocityGps < VelocityAllow + 35 THEN 1 ELSE 0 END) SpeedVioLevel3,
								SUM(CASE WHEN VelocityAllow + 35 <= VelocityGps THEN 1 ELSE 0 END) SpeedVioLevel4,
								SUM(CASE WHEN VelocityAllow + 5 <= VelocityGps THEN 1 ELSE 0 END) TotalSpeedVio,
								SUM(EndKm - StartKm) TotalKmVio,
								SUM(DATEDIFF(minute, StartTime, EndTime)) TotalTimeVio,
								FK_VehicleID
							FROM [BGT.SpeedOvers] WITH(NOLOCK)
							WHERE CreatedDate BETWEEN @FromDate AND @ToDate 
								  AND VelocityAllow + 5 <= VelocityGps
							GROUP BY FK_VehicleID
							) spo
						INNER JOIN (
							SELECT vhc.PK_VehicleID, vhc.FK_CompanyID, vhc.PrivateCode, tpType.DisplayName, vhc.VehiclePlate
							FROM [Vehicle.Vehicles] vhc WITH(NOLOCK)
							INNER JOIN [BGT.VehicleTransportTypes] vhcType WITH(NOLOCK) ON vhc.PK_VehicleID = vhcType.FK_VehicleID
							LEFT JOIN [BGT.TranportTypes] tpType WITH(NOLOCK) ON vhcType.FK_TransportTypeID = tpType.PK_TransportTypeID
							WHERE vhc.IsDeleted = 0 AND  vhc.FK_CompanyID = @CompanyID
								  AND ( @Count = 0 OR vhcType.FK_VehicleID in @ListVhcId)
						) vhcInfo ON spo.FK_VehicleID = vhcInfo.PK_VehicleID
						LEFT JOIN (
							SELECT
								FK_VehicleID,
								FK_CompanyID,
								SUM(TotalKmGps) TotalKm,
								SUM(ActivityTime) TotalTime
							FROM [Report.ActivitySummaries] WITH(NOLOCK)
							--WHERE FK_Date BETWEEN @FromDate AND @ToDate
							GROUP BY FK_VehicleID, FK_CompanyID
						) atvs ON vhcInfo.PK_VehicleID = atvs.FK_VehicleID AND vhcInfo.FK_CompanyID = atvs.FK_CompanyID
						";

            using (var connection = _dapperContext.CreateConnection("ServerLab3"))
            {
                var result = await connection.QueryAsync<GetAllSpeedViolationVehicleDto>(query, new
                {
                    input.FromDate,
                    input.ToDate,
                    input.ListVhcId,
					Count = input.ListVhcId.Count(),
                    CompanyID = input.CompanyId
                });
                return result.ToList();
            }
        }
    }
}
