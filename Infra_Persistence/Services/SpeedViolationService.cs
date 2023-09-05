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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infra_Persistence.Services
{
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

        public async Task<List<GetVehicleListDto>> GetVehicleByCompanyId(int input)
        {
            var result = await _unitOfWork.SpeedViolationRepository.GetVehicleByCompanyId(input);
            return result;
        }

        public async Task<PagedResultDto> GetAllSpeedViolationVehicle(SpeedViolationVehicleInput input)
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
                    var vhcList = await GetSpeedViolationVehicle(input);

                    totalCount = vhcList.Count();
                    result = vhcList.Skip((input.Page - 1) * input.PageSize).Take(input.PageSize).ToList();
                    AddEnumerableToSortedSet(cacheKey, vhcList);
                    _cache.KeyExpire(cacheKey, DateTime.Now.AddMinutes(5));
                }

                return new PagedResultDto
                {
                    TotalCount = totalCount,
                    ListVehicle = result
                };

            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                var valueDefault = new PagedResultDto();
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


        private async Task<List<GetAllSpeedViolationVehicleDto>> GetSpeedViolationVehicle(SpeedViolationVehicleInput input)
        {
            var result = await _unitOfWork.SpeedViolationRepository.GetAllSpeedViolationVehicle(input);
            return result;

        }
    }
}
