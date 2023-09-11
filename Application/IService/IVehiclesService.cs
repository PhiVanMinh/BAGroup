using Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IVehiclesService
    {
        Task<IEnumerable<Vehicles>> GetAllByCompany(int input);
    }
}
