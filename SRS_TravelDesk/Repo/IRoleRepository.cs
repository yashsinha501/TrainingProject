using SRS_TravelDesk.Models.Entities;

namespace SRS_TravelDesk.Repo
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(int roleId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
    }
}
