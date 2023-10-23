

using Application.IService;
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts;
using CachingFramework.Redis.Contracts.RedisObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Infra_Persistence.Helper
{
    /// <summary>Xử lý dữ liệu RedisCache</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/19/2023   created
    /// </Modified>
    public class RedisCacheHelper : IRedisCacheHelper
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<RedisCacheHelper> _logger;
        private readonly IDatabase _cache;

        public RedisCacheHelper( IConfiguration configuration, ILogger<RedisCacheHelper> logger )
        {
            _configuration = configuration;
            _logger = logger;
            var redis = ConnectionMultiplexer.Connect($"{_configuration["RedisCacheUrl"]},abortConnect=False");
            _cache = redis.GetDatabase();
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
            try
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

                _cache.KeyExpire(key, DateTime.Now.AddMinutes(5));
            } catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        /// <summary>Lấy dữ liệu từ bên cache</summary>
        /// <typeparam name="T">Kiểu dữ liệu cần trả</typeparam>
        /// <param name="link">Link api</param>
        /// <returns>Danh sách dữ liệu</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public Task<List<T>> GetDataFromCache<T>(string cacheKey, int page, int pageSize)
        {
            var result = new List<T>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var cachedData = _cache.KeyExists(cacheKey);
                    if (cachedData)
                    {
                        RedisValue[] redisData = { };
                        if (page == 0  && pageSize == 0)
                        {
                             redisData = _cache.SortedSetRangeByScore(cacheKey);
                        } else
                        {
                             redisData = _cache.SortedSetRangeByScore(cacheKey, (page - 1) * pageSize + (page == 1 ? 0 : 1), (page) * pageSize);
                        }
                        result = redisData.Select(d => JsonSerializer.Deserialize<T>(d)).ToList();

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetDataFromCache_{cacheKey}_{ex.Message}");
            }

            return Task.FromResult(result);
        }
    }
}
