using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBank.Backend.Interfaces;
using BloodBank.Backend.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace BloodBank.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("User data: {Request}", request);
            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Role = request.Role,
                    Email = request.Email,
                    OrganisationName = request.OrganisationName,
                    HospitalName = request.HospitalName,
                    Website = request.Website,
                    Address = request.Address,
                    Phone = request.Phone
                };

                await _userService.Register(user, request.Password);
                return Ok(new { success = true, message = "User registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokens = await _userService.Login(request.Email, request.Password);
            if (tokens == null)
            {
                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }
            _logger.LogInformation("TOKEN: {Tokens}", tokens);
            return Ok(new { success = true, token = tokens, message = "Login successful" });
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            _logger.LogInformation("Attempting to get current user.");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("USER ID: {userId}", userIdClaim);

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            var user = await _userService.GetCurrentUserAsync(userId);
            _logger.LogInformation("USER DATA: {user}", user);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            return Ok(new { success = true, user });
        }
    }

    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OrganisationName { get; set; }
        public string HospitalName { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
