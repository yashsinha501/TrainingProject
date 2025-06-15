using Microsoft.IdentityModel.Tokens;
using SRS_TravelDesk.Data;
using SRS_TravelDesk.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SRS_TravelDesk.Service
{
    public class JwtService
    {
        private readonly ApplicationDbContext _DbContext;
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration, ApplicationDbContext DbContext)
        {
            _configuration = configuration;
            _DbContext = DbContext;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim("Email",user.Email ?? ""),
            new Claim("Role", user.Role?.Name ?? "User"),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"] ?? "")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
