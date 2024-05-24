using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BloodBank.Backend.Interfaces;
using BloodBank.Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace BloodBank.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly string _connectionString;

        public UserService(IConfiguration configuration, ILogger<UserService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> Register(User user, string password)
        {
            _logger.LogInformation("Registering user with email: {Email}", user.Email);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", user.Email);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            if (count > 0)
            {
                _logger.LogInformation("A user with this email already exists: {Email}", user.Email);
                throw new InvalidOperationException("A user with this email already exists.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            query = @"
                INSERT INTO Users (Name, Role, Email, PasswordHash, OrganisationName, HospitalName, Website, Address, Phone)
                VALUES (@Name, @Role, @Email, @PasswordHash, @OrganisationName, @HospitalName, @Website, @Address, @Phone)";
            using var insertCommand = new MySqlCommand(query, connection);
            insertCommand.Parameters.AddWithValue("@Name", user.Name);
            insertCommand.Parameters.AddWithValue("@Role", user.Role);
            insertCommand.Parameters.AddWithValue("@Email", user.Email);
            insertCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            insertCommand.Parameters.AddWithValue("@OrganisationName", user.OrganisationName);
            insertCommand.Parameters.AddWithValue("@HospitalName", user.HospitalName);
            insertCommand.Parameters.AddWithValue("@Website", user.Website);
            insertCommand.Parameters.AddWithValue("@Address", user.Address);
            insertCommand.Parameters.AddWithValue("@Phone", user.Phone);

            await insertCommand.ExecuteNonQueryAsync();

            _logger.LogInformation("User registered successfully with email: {Email}", user.Email);
            return user;
        }

        public async Task<string> Login(string email, string password)
        {
            _logger.LogInformation("Attempting login for email: {Email}", email);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, PasswordHash, Role FROM Users WHERE Email = @Email";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                _logger.LogWarning("User not found with email: {Email}", email);
                return null;
            }

            var userId = reader.GetInt32("Id");
            var passwordHash = reader.GetString("PasswordHash");
            var role = reader.GetString("Role");

            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
            {
                _logger.LogWarning("Invalid password for email: {Email}", email);
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
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

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, Name, Email, Role, OrganisationName, HospitalName, Website, Address, Phone FROM Users WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                _logger.LogWarning("User not found with ID: {UserId}", userId);
                return null;
            }

            return new User
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Email = reader.GetString("Email"),
                Role = reader.GetString("Role"),
                OrganisationName = reader.GetString("OrganisationName"),
                HospitalName = reader.GetString("HospitalName"),
                Website = reader.GetString("Website"),
                Address = reader.GetString("Address"),
                Phone = reader.GetString("Phone")
            };
        }

        public async Task<UserDTO> GetCurrentUserAsync(int userId)
        {
            var user = await GetCurrentUser(userId);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
