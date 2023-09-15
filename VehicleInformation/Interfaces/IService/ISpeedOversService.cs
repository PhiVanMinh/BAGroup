using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Models;

namespace VehicleInformation.Interfaces.IService
{
    public interface ISpeedOversService
    {
        Task<List<BGT_SpeedOvers>> GetAllSpeedOversByDate(DateTime fromDate, DateTime toDate);
    }
}
