using Services.Common.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IDataService
    {
        Task<IEnumerable<BGT_TranportTypes>> GetTransportTypes(HttpClient httpClient);
        Task<IEnumerable<BGT_VehicleTransportTypes>> GetVehicleTransportType(HttpClient httpClient);
        Task<IEnumerable<Vehicle_Vehicles>> GetVehicleInfo(int input, HttpClient httpClient);
        Task<IEnumerable<Report_ActivitySummaries>> GetActivitySummaries(int input, HttpClient httpClient);
        Task<IEnumerable<BGT_SpeedOvers>> GetSpeedOvers(DateTime fromDate, DateTime toDate, HttpClient httpClient);

    }
}
