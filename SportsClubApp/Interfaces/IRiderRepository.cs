using SportsClubApp.Models;

namespace SportsClubApp.Interfaces
{
    public interface IRiderRepository
    {
        Task<List<Rider>> GetAllAsync();
        Task<Rider?> GetByIdAsync(int id);
        Task AddAsync(Rider rider);
        Task UpdateAsync(Rider rider);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
