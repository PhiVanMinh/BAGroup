using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Models;

namespace VehicleInformation.Interfaces.IService
{
    public interface ITransportTypesService
    {
        Task<List<BGT_TranportTypes>> GetAll();
    }
}
