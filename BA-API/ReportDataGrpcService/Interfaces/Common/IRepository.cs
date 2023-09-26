namespace ReportDataGrpcService.Interfaces.Common
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(T entity);

        IEnumerable<T> GetAllByDate(string columnName, DateTime fromDate, DateTime toDate, bool hasIsDeleted);
        IEnumerable<T> GetAllByColumnId(string columnName, int value, bool hasIsDeleted);
    }
}
