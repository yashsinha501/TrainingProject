using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Repo;

namespace SRS_TravelDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepository.AuthenticateAsync(dto.Email, dto.Password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Please enter the correct Email & Password"
                });
            }

            return Ok(new LoginResponseDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleName = user.Role?.Name
            });
        }
    }
}
