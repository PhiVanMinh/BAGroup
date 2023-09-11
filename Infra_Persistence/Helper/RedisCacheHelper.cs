

using Application.IService;
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts;
using CachingFramework.Redis.Contracts.RedisObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra_Persistence.Helper
{
    public class RedisCacheHelper : IRedisCacheHelper
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<RedisCacheHelper> _logger;

        public RedisCacheHelper( IConfiguration configuration, ILogger<RedisCacheHelper> logger )
        {
            _configuration = configuration;
            _logger = logger;
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
            } catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
    }
}
