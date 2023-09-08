using Application.Dto.SpeedViolation;
using Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IVehiclesRepository
    {
        Task<IEnumerable<Vehicles>> GetAllByCompany(SpeedViolationVehicleInput input);
    }
}
