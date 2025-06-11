namespace SRS_TravelDesk.Models.DTO
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? ManagerName { get; set; }
        public string? RoleName { get; set; }
    }
}
