using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportSpeedOver.API.Common.Interfaces.IHelper
{
    public interface IRedisCacheHelper
    {
        void AddEnumerableToSortedSet<T>(string key, IEnumerable<T> data);
        Task<List<T>> GetDataFromCache<T>(string cacheKey, int page, int pageSize);
    }
}
