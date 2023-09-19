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
using Infra_Persistence.Helper;
using Nest;

namespace Infra_Persistence.Services
{
    /// <summary>Báo cáo vi phạm tốc độ</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/6/2023   created
    /// </Modified>
    public class SpeedViolationService : ISpeedViolationService
    {
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheHelper _cacheHelper;
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<SpeedViolationService> _logger;
        private readonly IDatabase _cache;

        public SpeedViolationService(
            IUnitOfWork unitOfWork,
            ILogger<SpeedViolationService> logger,
            IConfiguration configuration,
            IRedisCacheHelper cacheHelper
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _cacheHelper = cacheHelper;
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
            var valueReport = new PagedResultDto<GetAllSpeedViolationVehicleDto>();
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
                    result = redisData.Select(d => JsonSerializer.Deserialize<GetAllSpeedViolationVehicleDto>(d) ?? new GetAllSpeedViolationVehicleDto()).ToList();
                }
                else
                {
                    //input.ToDate = input.FromDate.Value.AddDays(60);
                    var vhcList = await GetSpeedViolationVehicle(input);

                    totalCount = vhcList.Count();
                    result = vhcList.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize).ToList();
                    _cacheHelper.AddEnumerableToSortedSet(cacheKey, vhcList);
                    _cache.KeyExpire(cacheKey, DateTime.Now.AddMinutes(5));
                }
                valueReport.TotalCount = totalCount;
                valueReport.Result = result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return valueReport;

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
            var result = new List<GetAllSpeedViolationVehicleDto>();
            try
            {
                var vehicleInfo = await GetVehicleInfomation(input);
                var activityGroup = await GetActivitySummaries(input);
                var speedGroup = await GetSpeedOvers(input);

                result =  ( from speed in speedGroup
                            join vhc in vehicleInfo on speed.VehicleID equals vhc.VehicleID
                            join atv in activityGroup on vhc.VehicleID equals atv.VehicleID into activityGroupJoin
                            from atv in activityGroupJoin.DefaultIfEmpty()
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
                                TotalTime = atv?.TotalTime
                            }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetSpeedViolationVehicle_{ex.Message}_{input.FromDate}_{input.ToDate}_{string.Join("_", input.ListVhcId)}");
            }
            return result;

        }

        /// <summary>Lấy dữ liệu từ bên service khác</summary>
        /// <typeparam name="T">Kiểu dữ liệu cần trả</typeparam>
        /// <param name="link">Link api</param>
        /// <returns>Danh sách dữ liệu</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        private async Task<IEnumerable<T>> GetDataFromOtherService<T>(string link)
        {
            var result = new List<T>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string cacheKey = $"{DateTime.Now.ToString("dd_MM_yyyy_hh")} {link}";
                    var cachedData = _cache.KeyExists(cacheKey);
                    if (cachedData)
                    {
                        var redisData = _cache.SortedSetRangeByScore(cacheKey);
                        result = redisData.Select(d => JsonSerializer.Deserialize<T>(d)).ToList();
                    }
                    else
                    {
                        var option = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var response = await httpClient.PostAsync(link, null);
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<List<T>>(apiResponse, option) ?? new List<T>();
                        _cacheHelper.AddEnumerableToSortedSet(cacheKey, result);
                        _cache.KeyExpire(cacheKey, DateTime.Now.AddMinutes(5));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetData_{link}_{ex.Message}");
            }

            return result;
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
            var result = new List<GetVehicleInfomationDto>();
            try
            {
                var tranportTypes = await GetDataFromOtherService<TranportTypes>("https://localhost:44333/Vehicles/transport-type");
                var vehicleTransportTypes = await GetDataFromOtherService<VehicleTransportTypes>("https://localhost:44333/Vehicles/vehicle-type");
                var vehicles = await GetDataFromOtherService<Vehicles>($"https://localhost:44333/Vehicles/vehicle?input={input.CompanyId}");


                result = ( from vhc in vehicles
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
                                   }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetVehicleInfomation_{ex.Message}_{input.FromDate}_{input.ToDate}_{string.Join("_", input.ListVhcId)}");
            }
            return result;
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
            var result = new List<GetActivitySummariesDto>();
            try
            {
                var activitySummaries = await GetDataFromOtherService<ActivitySummaries>($"https://localhost:44333/Vehicles/activiti-summary?input={input.CompanyId}");

                 result = activitySummaries.GroupBy(e => new { e.FK_VehicleID, e.FK_CompanyID })
                            .Select(e => new GetActivitySummariesDto
                            {
                                CompanyId = e.Key.FK_CompanyID,
                                VehicleID = e.Key.FK_VehicleID,
                                TotalKm = e.Sum(a => a.TotalKmGps),
                                TotalTime = e.Sum(a => a.ActivityTime)
                            }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetActivitySummaries_{ex.Message}_{input.FromDate}_{input.ToDate}_{string.Join("_", input.ListVhcId)}");
            }
            return result;

        }

        /// <summary>Thông tin quá tốc độ</summary>
        /// <param name="input">Điều kiện lọc</param>
        /// <returns>Thông tin quá tốc độ của phương tiện</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/13/2023   created
        /// </Modified>
        private async Task<IEnumerable<GetSpeedOversDto>> GetSpeedOvers(SpeedViolationVehicleInput input)
        {
            var result = new List<GetSpeedOversDto>();
            try 
            {
                input.ToDate = input.FromDate!.Value.AddDays(60);
                var speedOvers = await GetDataFromOtherService<SpeedOvers>($"https://localhost:44333/Vehicles/speedOver?fromDate={input.FromDate}&toDate={input.ToDate}");

                result = speedOvers.Where(e => e.VelocityAllow + 5 <= e.VelocityGps).GroupBy(e => e.FK_VehicleID)
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
                        }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetSpeedOvers_{ex.Message}_{input.FromDate}_{input.ToDate}_{string.Join("_", input.ListVhcId)}");
            }
            return result;

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
                    result = redisData.Select(d => JsonSerializer.Deserialize<GetAllSpeedViolationVehicleDto>(d) ?? new GetAllSpeedViolationVehicleDto()).ToList();
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
