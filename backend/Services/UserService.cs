// File: Services/UserService.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BloodBank.Backend.Data;
using BloodBank.Backend.Interfaces;
using BloodBank.Backend.Models;
// File: Services/UserService.cs
using Microsoft.Extensions.Logging; // Ensure this using directive is present

namespace BloodBank.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, IConfiguration configuration, ILogger<UserService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<User> Register(User user, string password)
        {
            _logger.LogInformation("Registering user with email: {Email}", user.Email);

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                _logger.LogInformation("A user with this email already exists.: {Email}", user.Email);

                throw new InvalidOperationException("A user with this email already exists.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User registered successfully with email: {Email}", user.Email);
            return user;
        }


        // File: Services/UserService.cs

        public async Task<string> Login(string email, string password)
        {
            _logger.LogInformation("Attempting login for email: {Email}", email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for email: {Email}", email);
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            _logger.LogInformation("Attempting login tokenHandler", tokenHandler);
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            _logger.LogInformation("Attempting login KEY", key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        public async Task<User> GetCurrentUser(int userId)
        {
            _logger.LogInformation("Fetching current user with ID: {UserId}", userId);
            return await _context.Users.FindAsync(userId);
        }
    }
}
