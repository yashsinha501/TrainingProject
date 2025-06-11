using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRS_TravelDesk.Models.Entities;
using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Repo;

namespace SRS_TravelDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;

        public UserController(IUserRepository userRepo, IRoleRepository roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepo.GetAllAsync();

            var response = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                EmployeeId = u.EmployeeId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                ManagerName = u.ManagerName,
                RoleName = u.Role?.Name
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            var dto = new UserResponseDto
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Department = user.Department,
                ManagerName = user.ManagerName,
                RoleName = user.Role?.Name
            };

            return Ok(dto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto dto)
        {
            var roleExists = await _roleRepo.RoleExistsAsync(dto.RoleId);
            if (!roleExists)
                return BadRequest("Invalid RoleId.");

            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict("Email already exists.");

            var user = new User
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Department = dto.Department,
                ManagerName = dto.ManagerName,
                RoleId = dto.RoleId,
                Password = dto.Password
            };

            var created = await _userRepo.CreateAsync(user);
            if (created == null)
                return StatusCode(500, "Failed to create user.");

            return CreatedAtAction(nameof(Get), new { id = created.Id }, new UserResponseDto
            {
                Id = created.Id,
                EmployeeId = created.EmployeeId,
                FirstName = created.FirstName,
                LastName = created.LastName,
                Email = created.Email,
                Department = created.Department,
                ManagerName = created.ManagerName,
                RoleName = created.Role?.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var updatedUser = new User
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Department = dto.Department,
                ManagerName = dto.ManagerName,
                RoleId = dto.RoleId,
                Password = dto.NewPassword
            };

            var updated = await _userRepo.UpdateAsync(id, updatedUser);
            if (updated == null) return NotFound();

            var dtoResponse = new UserResponseDto
            {
                Id = updated.Id,
                EmployeeId = updated.EmployeeId,
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                Email = updated.Email,
                Department = updated.Department,
                ManagerName = updated.ManagerName,
                RoleName = updated.Role?.Name
            };

            return Ok(dtoResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userRepo.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepo.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized("Invalid email or password.");

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Department = user.Department,
                ManagerName = user.ManagerName,
                RoleName = user.Role?.Name
            });
        }
    }
}

