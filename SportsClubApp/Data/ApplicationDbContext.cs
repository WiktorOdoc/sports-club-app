using Microsoft.EntityFrameworkCore;
using SportsClubApp.Models;

namespace SportsClubApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Rider> Riders { get; set; }

        public DbSet<Team> Teams { get; set; }
    }
}
