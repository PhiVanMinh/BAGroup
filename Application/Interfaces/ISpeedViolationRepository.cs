using Application.Dto.SpeedViolation;

namespace Application.Interfaces
{
    public interface ISpeedViolationRepository
    {
        Task<List<GetAllSpeedViolationVehicleDto>> GetAllSpeedViolationVehicle(SpeedViolationVehicleInput input);
        Task<List<GetVehicleListDto>> GetVehicleByCompanyId(int input);
    }
}
