using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Data;
using SRS_TravelDesk.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace SRS_TravelDesk.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            user.Password = HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
        }

        public async Task<User> UpdateAsync(int id, User updatedUser)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return null;

            existing.EmployeeId = updatedUser.EmployeeId;
            existing.FirstName = updatedUser.FirstName;
            existing.LastName = updatedUser.LastName;
            existing.Email = updatedUser.Email;
            existing.Department = updatedUser.Department;
            existing.ManagerName = updatedUser.ManagerName;
            existing.RoleId = updatedUser.RoleId;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                existing.Password = HashPassword(updatedUser.Password);

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var hash = HashPassword(password);
            return await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hash);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
