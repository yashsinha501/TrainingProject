namespace SRS_TravelDesk.Models.DTO
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
    }
}
