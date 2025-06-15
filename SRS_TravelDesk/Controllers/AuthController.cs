using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Repo;
using SRS_TravelDesk.Services;

namespace SRS_TravelDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        public AuthController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }


            var user = await _userRepository.AuthenticateAsync(dto.Email, dto.Password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Please enter the correct Email & Password"
                });
            }
            var token = _jwtService.GenerateToken(user);
            return Ok(new LoginResponseDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleName = user.Role?.Name,
                Token = token
            });
        }
    }
}
