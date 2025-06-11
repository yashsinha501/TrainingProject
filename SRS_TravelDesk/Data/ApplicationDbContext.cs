using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> user {  get; set; }
    }
}
