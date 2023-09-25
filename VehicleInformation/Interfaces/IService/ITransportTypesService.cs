using Services.Common.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleInformation.Interfaces.IService
{
    public interface ITransportTypesService
    {
        Task<List<BGT_TranportTypes>> GetAll();
    }
}
