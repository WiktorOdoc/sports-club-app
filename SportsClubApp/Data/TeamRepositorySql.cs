
using Microsoft.EntityFrameworkCore;
using SportsClubApp.Interfaces;
using SportsClubApp.Models;

namespace SportsClubApp.Data
{
    public class TeamRepositorySql : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepositorySql(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Team>> GetAllAsync()
        {
            return await _context.Teams
                .Include(t => t.Riders)
                .ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Riders)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Team team)
        {
            var existingTeam = await _context.Teams
                .FindAsync(team.Id);

            if (existingTeam == null)
                return;

            existingTeam.Name = team.Name;
            existingTeam.City = team.City;
            existingTeam.League = team.League;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team != null)
            {
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teams
                .AnyAsync(t => t.Id == id);
        }
    }
}
