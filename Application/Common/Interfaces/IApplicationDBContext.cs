

namespace Application.Common.Interfaces
{
    public interface IApplicationDBContext
    {
        Task<int> SaveChangesAsync();
    }
}
