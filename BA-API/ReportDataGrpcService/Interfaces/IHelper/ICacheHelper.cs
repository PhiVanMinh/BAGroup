namespace ReportDataGrpcService.Interfaces.IHelper
{
    public interface ICacheHelper
    {
        void AddEnumerableToSortedSet<T>(string key, IEnumerable<T> data);
        Task<List<T>> GetDataFromCache<T>(string cacheKey, int page, int pageSize);
    }
}
