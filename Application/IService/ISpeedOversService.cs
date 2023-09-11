using Application.Model;

namespace Application.IService
{
    public interface ISpeedOversService
    {
        Task<IEnumerable<SpeedOvers>> GetAllSpeedOversByDate(DateTime? fromDate, DateTime? toDate);
    }
}
