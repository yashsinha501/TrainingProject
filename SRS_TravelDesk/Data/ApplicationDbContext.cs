using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CommentedBy)
                .WithMany()
                .HasForeignKey(c => c.CommentedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TravelRequest>()
                .HasOne(tr => tr.RequestedBy)
                .WithMany(u => u.TravelRequests)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrator" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "Employee" },
                new Role { Id = 4, Name = "TravelHr" }
            );
        }
    }
}
