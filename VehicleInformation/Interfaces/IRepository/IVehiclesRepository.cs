using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Common.Interfaces;
using VehicleInformation.Models;

namespace VehicleInformation.Interfaces.IRepository
{
    public interface IVehiclesRepository: IGenericRepository<Vehicle_Vehicles>
    {
        //Task<IEnumerable<Vehicles>> GetAllByCompany(int input);
    }
}
