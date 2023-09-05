using Application.Dto.SpeedViolation;
using Application.Dto.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface ISpeedViolationService
    {
        public Task<PagedResultDto> GetAllSpeedViolationVehicle(SpeedViolationVehicleInput input);
        public Task<List<GetVehicleListDto>> GetVehicleByCompanyId(int input);
    }
}
