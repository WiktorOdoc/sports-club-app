
using Microsoft.EntityFrameworkCore;
using SportsClubApp.Interfaces;
using SportsClubApp.Models;

namespace SportsClubApp.Data
{
    public class RiderRepositorySql : IRiderRepository
    {
        private readonly ApplicationDbContext _context;

        public RiderRepositorySql(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rider>> GetAllAsync()
        {
            return await _context.Riders.ToListAsync();
        }

        public async Task<Rider?> GetByIdAsync(int id)
        {
            return await _context.Riders.FindAsync(id);
        }

        public async Task AddAsync(Rider rider)
        {
            _context.Riders.Add(rider);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rider rider)
        {
            var existingRider =
                await _context.Riders.FindAsync(rider.Id);

            if (existingRider == null)
                return;

            existingRider.FirstName = rider.FirstName;
            existingRider.LastName = rider.LastName;
            existingRider.BirthYear = rider.BirthYear;
            existingRider.RacingNumber = rider.RacingNumber;
            existingRider.TeamId = rider.TeamId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rider = await _context.Riders.FindAsync(id);

            if (rider != null)
            {
                _context.Riders.Remove(rider);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var riders = await _context.Riders.AnyAsync(r => r.Id == id);
            return riders;
        }
    }
}
