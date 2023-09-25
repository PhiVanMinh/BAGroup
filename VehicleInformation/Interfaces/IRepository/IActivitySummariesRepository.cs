using Services.Common.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Common.Interfaces;

namespace VehicleInformation.Interfaces.IRepository
{
    public interface IActivitySummariesRepository : IGenericRepository<Report_ActivitySummaries>
    {
    }
}
