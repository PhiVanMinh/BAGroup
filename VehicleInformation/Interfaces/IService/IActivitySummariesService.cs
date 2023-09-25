using Services.Common.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleInformation.Interfaces.IService
{
    public interface IActivitySummariesService
    {
        Task<List<Report_ActivitySummaries>> GetAllByCompany(int input);
    }
}
