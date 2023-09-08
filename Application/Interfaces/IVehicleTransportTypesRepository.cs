﻿using Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IVehicleTransportTypesRepository
    {
        Task<IEnumerable<VehicleTransportTypes>> GetAll();
    }
}
