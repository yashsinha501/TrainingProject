using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SRS_TravelDesk.Models.DTO;
using SRS_TravelDesk.Models.Entities;
using SRS_TravelDesk.Repo;
using System.Security.Claims;

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
                RoleName = u.Role?.Name,
                Manager = u.Manager != null ? new ManagerDto
                {
                    Id = u.Manager.Id,
                    EmployeeId = u.Manager.EmployeeId,
                    FirstName = u.Manager.FirstName,
                    LastName = u.Manager.LastName,
                    Email = u.Manager.Email,
                    Department = u.Manager.Department,
                    RoleName = u.Manager.Role?.Name
                } : null
            });

            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var u = await _userRepo.GetByIdAsync(id);
            if (u == null) return NotFound();

            var response =  new UserResponseDto
            {
                Id = u.Id,
                EmployeeId = u.EmployeeId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Department = u.Department,
                RoleName = u.Role?.Name,
                Manager = u.Manager != null ? new ManagerDto
                {
                    Id = u.Manager.Id,
                    EmployeeId = u.Manager.EmployeeId,
                    FirstName = u.Manager.FirstName,
                    LastName = u.Manager.LastName,
                    Email = u.Manager.Email,
                    Department = u.Manager.Department,
                    RoleName = u.Manager.Role?.Name
                } : null
            };

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto dto)
        {
            // Validate role
            var roleExists = await _roleRepo.RoleExistsAsync(dto.RoleId);
            if (!roleExists)
                return BadRequest("Invalid RoleId.");

            // Check for duplicate email
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict("Email already exists.");

            
            User manager = null;
            if (dto.ManagerId != null)
            {
                manager = await _userRepo.GetByIdAsync(dto.ManagerId.Value);
                if (manager == null)
                    return BadRequest("ManagerId is invalid.");
            }

            // Create user
            var user = new User
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Department = dto.Department,
                RoleId = dto.RoleId,
                ManagerId = dto.ManagerId,
                Password = dto.Password 
            };

            var created = await _userRepo.CreateAsync(user);
            if (created == null)
                return StatusCode(500, "Failed to create user.");

            
            created = await _userRepo.GetByIdAsync(created.Id);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, new UserResponseDto
            {
                Id = created.Id,
                EmployeeId = created.EmployeeId,
                FirstName = created.FirstName,
                LastName = created.LastName,
                Email = created.Email,
                Department = created.Department,
                RoleName = created.Role?.Name,
                Manager = created.Manager != null ? new ManagerDto
                {
                    Id = created.Manager.Id,
                    EmployeeId = created.Manager.EmployeeId,
                    FirstName = created.Manager.FirstName,
                    LastName = created.Manager.LastName,
                    Email = created.Manager.Email,
                    Department = created.Manager.Department,
                    RoleName = created.Manager.Role?.Name
                } : null
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            // Optional: Validate manager ID if needed
            if (dto.ManagerId != null)
            {
                var manager = await _userRepo.GetByIdAsync(dto.ManagerId.Value);
                if (manager == null)
                    return BadRequest("Invalid ManagerId.");
            }

            var updatedUser = new User
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Department = dto.Department,
                ManagerId = dto.ManagerId,
                RoleId = dto.RoleId,
                Password = dto.NewPassword
            };

            var updated = await _userRepo.UpdateAsync(id, updatedUser);
            if (updated == null) return NotFound();

            
            updated = await _userRepo.GetByIdAsync(id);

            var dtoResponse = new UserResponseDto
            {
                Id = updated.Id,
                EmployeeId = updated.EmployeeId,
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                Email = updated.Email,
                Department = updated.Department,
                RoleName = updated.Role?.Name,
                Manager = updated.Manager != null ? new ManagerDto
                {
                    Id = updated.Manager.Id,
                    EmployeeId = updated.Manager.EmployeeId,
                    FirstName = updated.Manager.FirstName,
                    LastName = updated.Manager.LastName,
                    Email = updated.Manager.Email,
                    Department = updated.Manager.Department,
                    RoleName = updated.Manager.Role?.Name
                } : null
            };

            return Ok(dtoResponse);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userRepo.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("secure-data")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                Id = userId,
                Username = username,
                Role = role
            });
        }

    }
}

