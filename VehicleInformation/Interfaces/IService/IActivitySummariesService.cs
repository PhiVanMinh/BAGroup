﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleInformation.Models;

namespace VehicleInformation.Interfaces.IService
{
    public interface IActivitySummariesService
    {
        Task<List<Report_ActivitySummaries>> GetAllByCompany(int input);
    }
}
