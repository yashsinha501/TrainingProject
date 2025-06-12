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

        public DbSet<User> Users {  get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Comment → User (CommentedBy)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CommentedBy)
                .WithMany()
                .HasForeignKey(c => c.CommentedByUserId)
                .OnDelete(DeleteBehavior.NoAction); // ← VERY IMPORTANT

            // Comment → TravelRequest
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TravelRequest)
                .WithMany(tr => tr.Comments)
                .HasForeignKey(c => c.TravelRequestId)
                .OnDelete(DeleteBehavior.NoAction); // ← SAFER

            // TravelRequest → User (if exists)
            modelBuilder.Entity<TravelRequest>()
                .HasOne(tr => tr.RequestedBy)
                .WithMany() 
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrator" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "Employee" },
                new Role { Id = 4, Name = "TravelHr" }
            );
        }

    }
}
