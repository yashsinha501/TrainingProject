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

        public DbSet<User> user {  get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }

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
                .HasOne(tr => tr.User)
                .WithMany() // or WithMany(u => u.TravelRequests)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.NoAction); // ← FIX THIS TOO IF EXISTS
        }

    }
}
