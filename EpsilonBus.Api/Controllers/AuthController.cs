using EpsilonBus.Api.Data;
using EpsilonBus.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EpsilonBus.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly EpsilonBusDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(EpsilonBusDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || user.PasswordHash != request.Password) // Replace with hash check in production
                return Unauthorized();

            var token = GenerateJwtToken(user);
            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost("validate-employee")]
        [Authorize]
        public async Task<ActionResult<ValidateEmployeeResponse>> ValidateEmployee()
        {
            // Extract EntraIDCode from token (e.g., oid claim)
            var entraIdCode = User.FindFirst("oid")?.Value;
            if (string.IsNullOrEmpty(entraIdCode))
                return Unauthorized();

            // Find employee by EntraIDCode
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EntraIDCode == entraIdCode);
            if (employee == null)
                return NotFound();

            // Check Users table for special role
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EntraIDCode == entraIdCode);
            var role = user?.Role ?? "user";

            return Ok(new ValidateEmployeeResponse
            {
                EmployeeId = employee.ID,
                EntraIDCode = employee.EntraIDCode,
                EmployeeName = employee.EmployeeName,
                Role = role
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.ID.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "user") // Add role claim
            };

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
