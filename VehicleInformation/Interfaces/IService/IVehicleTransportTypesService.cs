using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Models;

namespace VehicleInformation.Interfaces.IService
{
    public interface IVehicleTransportTypesService
    {
        Task<List<BGT_VehicleTransportTypes>> GetAll();
    }
}
