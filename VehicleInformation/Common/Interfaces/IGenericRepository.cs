using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleInformation.Common.Interfaces
{
    public interface IGenericRepository<T> where T : class
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
