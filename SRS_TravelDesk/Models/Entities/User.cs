using System.Data;

namespace SRS_TravelDesk.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Department { get; set; }
        public string ManagerName { get; set; }
        public int RoleId { get; set; }

        public Role Role { get; set; }
        public ICollection<TravelRequest> TravelRequests { get; set; }
    }

}
