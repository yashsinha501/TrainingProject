using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Models.Entities;
using System.Security.Cryptography;

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

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //user has a manager
            modelBuilder.Entity<User>()
           .HasOne(u => u.Manager)
           .WithMany(m => m.Subordinates)
           .HasForeignKey(u => u.ManagerId)
           .OnDelete(DeleteBehavior.Restrict);

            // Comment → User (CommentedBy)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CommentedBy)
                .WithMany()
                .HasForeignKey(c => c.CommentedByUserId)
                .OnDelete(DeleteBehavior.NoAction); 
            // Comment → TravelRequest
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TravelRequest)
                .WithMany(tr => tr.Comments)
                .HasForeignKey(c => c.TravelRequestId)
                .OnDelete(DeleteBehavior.NoAction);
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

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                EmployeeId = "EMP-0001",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@srs.com",
                Password = HashPassword("Admin@123"), 
                Department = "Administration",
                RoleId = 1, 
                ManagerId = null
            });
        }

    }
}
