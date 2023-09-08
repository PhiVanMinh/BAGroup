using Application.Dto.SpeedViolation;
using Application.Dto.Users;
using Application.Interfaces;
using Application.IService;
using CachingFramework.Redis.Contracts.RedisObjects;
using CachingFramework.Redis.Contracts;
using CachingFramework.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.ComponentModel.Design;
using Application.Model;

namespace Infra_Persistence.Services
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/6/2023   created
    /// </Modified>
    public class SpeedViolationService : ISpeedViolationService
    {
        private readonly IConfiguration _configuration;
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeedViolationService> _logger;
        private readonly IDatabase _cache;

        public SpeedViolationService(
            IUnitOfWork unitOfWork,
            ILogger<SpeedViolationService> logger,
            IConfiguration configuration
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;

            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
        }

        /// <summary>Lấy dánh sách các xe theo thông tin công ty</summary>
        /// <param name="input">Mã công ty</param>
        /// <returns>Danh sách các xe theo công ty</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/6/2023   created
        /// </Modified>
        public async Task<List<GetVehicleListDto>> GetVehicleByCompanyId(int input)
        {
            try
            {
                var result = await _unitOfWork.SpeedViolationRepository.GetVehicleByCompanyId(input);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new List<GetVehicleListDto>();
                return valueDefault;
            }
        }

        /// <summary>Lấy danh sách các xe vi phạm tốc độ</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Danh sách xe vi phạm theo điều kiện lọc</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/6/2023   created
        /// </Modified>
        public async Task<PagedResultDto<GetAllSpeedViolationVehicleDto>> GetAllSpeedViolationVehicle(SpeedViolationVehicleInput input)
        {
            string cacheKey = $"{DateTime.Now.ToString("dd_MM_yyyy_hh")} {input.FromDate}_{input.ToDate}_{string.Join("_",input.ListVhcId)}";
            try
            {
                List<GetAllSpeedViolationVehicleDto> result = new List<GetAllSpeedViolationVehicleDto>();
                var totalCount = 0;

                var cachedData = _cache.KeyExists(cacheKey);
                if (cachedData)
                {
                    totalCount = (int)_cache.SortedSetLength($"{cacheKey}");
                    var redisData = _cache.SortedSetRangeByScore(cacheKey, (input.Page - 1) * input.PageSize, (input.Page) * input.PageSize - (input.Page == 1 ? 0 : 1));
                    result = redisData.Select(d => JsonSerializer.Deserialize<GetAllSpeedViolationVehicleDto>(d)).ToList();
                }
                else
                {
                    //input.ToDate = input.FromDate.Value.AddDays(60);
                    var vhcList = await GetSpeedViolationVehicle(input);

                    totalCount = vhcList.Count();
                    result = vhcList.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize).ToList();
                    AddEnumerableToSortedSet(cacheKey, vhcList);
                    _cache.KeyExpire(cacheKey, DateTime.Now.AddMinutes(5));
                }
                return new PagedResultDto<GetAllSpeedViolationVehicleDto>
                {
                    TotalCount = totalCount,
                    Result = result
                };

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new PagedResultDto<GetAllSpeedViolationVehicleDto>();
                return valueDefault;
            }
        }

        /// <summary>Thêm dữ liệu và redis cache</summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="key">Key cache để sau tìm kiếm</param>
        /// <param name="data">Danh sách dữ liệu cần lưu redis</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/22/2023   created
        /// </Modified>
        public void AddEnumerableToSortedSet<T>(string key, IEnumerable<T> data)
        {
            int i = 1;
            var context = new RedisContext(_configuration["RedisCacheUrl"]);
            IRedisSortedSet<T> sortedSet = context.Collections.GetRedisSortedSet<T>(key);
            sortedSet.AddRange(data.Select((m) => new SortedMember<T>(i, m)
            {
                Value = m,
                Score = i++
            }));
            context.Dispose();
        }

        /// <summary>Lấy thông tin vi phạm của các xe</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Thông tin vi phạm của các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/8/2023   created
        /// </Modified>
        private async Task<List<GetAllSpeedViolationVehicleDto>> GetSpeedViolationVehicle(SpeedViolationVehicleInput input)
        {
            var vehicleInfo = await GetVehicleInfomation(input);
            var activityGroup = await GetActivitySummaries(input);
            var speedGroup = await GetSpeedOvers(input);

            var test = from speed in speedGroup
                       join vhc in vehicleInfo on speed.VehicleID equals vhc.VehicleID
                       select speed;

            var abc = test.Count();

            var speedVioVehicles = from speed in speedGroup
                                   join vhc in vehicleInfo on speed.VehicleID equals vhc.VehicleID
                                   join atv in activityGroup on vhc.VehicleID equals atv.VehicleID into activityGroupJoin from atv in activityGroupJoin.DefaultIfEmpty()
                                   orderby speed.VehicleID
                                   select new GetAllSpeedViolationVehicleDto
                                   {
                                     VehicleID = vhc.VehicleID,
                                     VehiclePlate = vhc.VehiclePlate,
                                     PrivateCode = vhc.PrivateCode,
                                     TransportType = vhc.TransportType,
                                     SpeedVioLevel1 = speed.SpeedVioLevel1,
                                     SpeedVioLevel2 = speed.SpeedVioLevel2,
                                     SpeedVioLevel3 = speed.SpeedVioLevel3,
                                     SpeedVioLevel4 = speed.SpeedVioLevel4,
                                     TotalSpeedVio = speed.TotalSpeedVio,
                                     RatioSpeedVio = (atv?.TotalKm != null && atv.TotalKm > 1000) ? (speed.TotalSpeedVio * 1000 / atv.TotalKm) : speed.TotalSpeedVio,
                                     TotalKmVio = speed.TotalKmVio,
                                     TotalKm = atv?.TotalKm,
                                     TotalTimeVio = speed.TotalTimeVio,
                                     TotalTime =  atv?.TotalTime
                                   };
            return speedVioVehicles.ToList();
        }

        /// <summary>Lấy thông tin các xe</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Thông tin các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/8/2023   created
        /// </Modified>
        private async Task<IEnumerable<GetVehicleInfomationDto>> GetVehicleInfomation(SpeedViolationVehicleInput input)
        {
            try
            {
                var tranportTypes = await _unitOfWork.TranportTypesRepository.GetAll();
                var vehicleTransportTypes = await _unitOfWork.VehicleTransportTypesRepository.GetAll();
                var vehicles = await _unitOfWork.VehiclesRepository.GetAllByCompany(input);

                IEnumerable<GetVehicleInfomationDto> result = from vhc in vehicles
                                                              join vhcType in vehicleTransportTypes on vhc.PK_VehicleID equals vhcType.FK_VehicleID
                                                              join type in tranportTypes on vhcType.FK_TransportTypeID equals type.PK_TransportTypeID
                                                              where (input.ListVhcId.Count() == 0 || input.ListVhcId.Contains(vhc.PK_VehicleID))
                                                              select new GetVehicleInfomationDto
                                                              {
                                                                  VehicleID = vhc.PK_VehicleID,
                                                                  CompanyID = vhc.FK_CompanyID,
                                                                  PrivateCode = vhc.PrivateCode,
                                                                  TransportType = type.DisplayName,
                                                                  VehiclePlate = vhc.VehiclePlate
                                                              };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new List<GetVehicleInfomationDto>();
                return valueDefault;
            }
        }

        /// <summary>Lấy thông tin tổng hợp hoạt động của các xe</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Thông tin tổng hợp của các xe</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/8/2023   created
        /// </Modified>
        private async Task<IEnumerable<GetActivitySummariesDto>> GetActivitySummaries(SpeedViolationVehicleInput input)
        {
            try
            {
                var activitySummaries = await _unitOfWork.ActivitySummariesRepository.GetAll(input);
                var result = activitySummaries.GroupBy(e => new { e.FK_VehicleID, e.FK_CompanyID })
                            .Select(e => new GetActivitySummariesDto
                            {
                                CompanyId = e.Key.FK_CompanyID,
                                VehicleID = e.Key.FK_VehicleID,
                                TotalKm = e.Sum(a => a.TotalKmGps),
                                TotalTime = e.Sum(a => a.ActivityTime)
                            });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new List<GetActivitySummariesDto>();
                return valueDefault;
            }
}

        private async Task<IEnumerable<GetSpeedOversDto>> GetSpeedOvers(SpeedViolationVehicleInput input)
        {
            try 
            { 
            var speedOvers = await _unitOfWork.SpeedOversRepository.GetAllByDate(input);
            var result = speedOvers.Where(e => e.VelocityAllow + 5 <= e.VelocityGps).GroupBy(e => e.FK_VehicleID)
                        .Select(e => new GetSpeedOversDto
                        {
                           VehicleID = e.Key,
                           SpeedVioLevel1 = e.Count(s => s.VelocityAllow + 5 <= s.VelocityGps && s.VelocityGps < s.VelocityAllow + 10),
                           SpeedVioLevel2 = e.Count(s => s.VelocityAllow + 10 <= s.VelocityGps && s.VelocityGps < s.VelocityAllow + 20),
                           SpeedVioLevel3 = e.Count(s => s.VelocityAllow + 20 <= s.VelocityGps && s.VelocityGps <= s.VelocityAllow + 35),
                           SpeedVioLevel4 = e.Count(s => s.VelocityAllow + 35 < s.VelocityGps),
                           TotalSpeedVio = e.Count(),
                           TotalKmVio = e.Sum(s => s.EndKm - s.StartKm),
                           TotalTimeVio = (int)Math.Round(e.Sum(s => (s.EndTime != null && s.StartTime != null) ? (s.EndTime - s.StartTime).Value.TotalMinutes : 0), 2)
                        });
            return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new List<GetSpeedOversDto>();
                return valueDefault;
            }
        }

        /// <summary>Lấy dữ liệu để xuất excel</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Danh sách người dùng</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    8/17/2023   created
        /// </Modified>
        public async Task<List<GetAllSpeedViolationVehicleDto>> GetDataToExportExcel(SpeedViolationVehicleInput input)
        {
            try
            {
                string cacheKey = $"{DateTime.Now.ToString("dd_MM_yyyy_hh")} {input.FromDate}_{input.ToDate}_{string.Join("_", input.ListVhcId)}";
                List<GetAllSpeedViolationVehicleDto> result = new List<GetAllSpeedViolationVehicleDto>();
                var totalCount = 0;

                var cachedData = _cache.KeyExists(cacheKey);
                if (cachedData)
                {
                    totalCount = (int)_cache.SortedSetLength($"{cacheKey}");
                    var redisData = _cache.SortedSetRangeByScore(cacheKey);
                    result = redisData.Select(d => JsonSerializer.Deserialize<GetAllSpeedViolationVehicleDto>(d)).ToList();
                }
                else
                {
                    var resultQuery = await GetSpeedViolationVehicle(input);
                    result = resultQuery.ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new List<GetAllSpeedViolationVehicleDto>();
                return valueDefault;
            }
        }

    }
}
