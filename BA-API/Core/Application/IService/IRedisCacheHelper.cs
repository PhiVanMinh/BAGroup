using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IRedisCacheHelper
    {
        void AddEnumerableToSortedSet<T>(string key, IEnumerable<T> data);
        Task<List<T>> GetDataFromCache<T>(string cacheKey, int page, int pageSize);
    }
}
