using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IHttpRequestHelper
    {
        Task<IEnumerable<T>> GetDataFromOtherService<T>(string link);
    }
}
