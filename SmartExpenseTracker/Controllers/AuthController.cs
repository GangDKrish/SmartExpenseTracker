using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartExpenseTracker.Data;
using SmartExpenseTracker.DTOs;
using SmartExpenseTracker.Models;
using System.Security.Cryptography;
using System.Text;

namespace SmartExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return Conflict(new { message = "A user with this email already exists." });

            var user = new User
            {
                Email = request.Email,
                Name = request.Name ?? string.Empty,
                PasswordHash = HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(CreateAuthResponse(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || user.PasswordHash != HashPassword(request.Password))
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(CreateAuthResponse(user));
        }

        private AuthResponseDto CreateAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SigningKey"] ?? throw new InvalidOperationException("JwtSettings:SigningKey is missing.")));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name)
            };

            var expirationMinutes = int.TryParse(jwtSettings["ExpirationMinutes"], out var parsedMinutes)
                ? parsedMinutes
                : 60;

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
