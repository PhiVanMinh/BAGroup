using Application.Dto.SpeedViolation;

namespace Application.Interfaces
{
    public interface ISpeedViolationRepository
    {
        Task<List<GetVehicleListDto>> GetVehicleByCompanyId(int input);
    }
}
