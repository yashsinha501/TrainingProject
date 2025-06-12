using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Repo
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email); 
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
        Task<User> AuthenticateAsync(string email, string password);
    }
}
