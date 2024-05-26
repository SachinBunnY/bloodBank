using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BloodBank.Backend.Interfaces;
using BloodBank.Backend.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Razorpay.Api;
using Payment = BloodBank.Backend.Models.Payment; // Alias for BloodBank Payment class
// using RazorpayPayment = Razorpay.Api.Payment;

namespace BloodBank.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        private readonly Data.ApplicationDbContext _context;
        private IConfiguration _config;

        public AuthController(IUserService userService, ILogger<AuthController> logger, Data.ApplicationDbContext context, IConfiguration config)
        {
            _userService = userService;
            _logger = logger;
            _context = context;
            _config = config;
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

                var registeredUser = await _userService.Register(user, request.Password);
                return Ok(new { success = true, message = "User registered successfully", user = registeredUser });
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
        // [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            _logger.LogInformation("Attempting to get current user.");
            var claims = User.Claims;
            _logger.LogInformation("Attempting to get current user:{User}", User);
            foreach (var claim in claims)
            {
                _logger.LogInformation("Claim Type: {type}, Claim Value: {value}", claim.Type, claim.Value);
            }

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


        // RAZOR PAYMENT INTEGRATION START FROM HERE
        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] PaymentRequest request)
        {
            try
            {
                var client = new RazorpayClient(_config["Razorpay:KeyId"], _config["Razorpay:KeySecret"]);
                var options = new Dictionary<string, object>
        {
            { "amount", request.Amount * 100 },
            { "currency", "INR" },
            { "receipt", "receipt#1" },
            { "payment_capture", 1 }
        };
                var order = client.Order.Create(options);

                var payment = new Payment
                {
                    OrderId = order["id"].ToString(),
                    Amount = request.Amount,
                    PaymentDate = DateTime.UtcNow,
                    Status = "Created"
                };
                _context.Payments.Add(payment);
                _context.SaveChanges();
                _logger.LogError("Successfully created order");
                return Ok(new { id = order["id"].ToString(), amount = request.Amount * 100 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new { message = "Error creating order" });
            }
        }

        [HttpPost("verify-payment")]
        public IActionResult VerifyPayment([FromBody] PaymentVerificationRequest request)
        {
            _logger.LogError("Verify-payment: {request}", request);
            var payment = _context.Payments.FirstOrDefault(p => p.OrderId == request.RazorpayOrderId);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found" });
            }
            _logger.LogError("Verify-payment payment: {payment}", payment);

            var isSignatureValid = VerifySignature(request.RazorpayOrderId, request.RazorpayPaymentId, request.RazorpaySignature);
            _logger.LogError("Verify-payment isSignatureValid: {isSignatureValid}", isSignatureValid);
            if (isSignatureValid)
            {
                payment.PaymentId = request.RazorpayPaymentId;
                payment.Signature = request.RazorpaySignature;
                payment.Status = "Success";
                _context.SaveChanges();
                return Ok(new { success = true, message = "Payment verified successfully" });
            }
            else
            {
                payment.Status = "Failed";
                _context.SaveChanges();
                return BadRequest(new { message = "Invalid signature" });
            }
        }

        private bool VerifySignature(string orderId, string paymentId, string signature)
        {
            var keySecret = _config["Razorpay:KeySecret"];
            var generatedSignature = $"{orderId}|{paymentId}";
            var generatedSignatureBytes = new System.Text.UTF8Encoding().GetBytes(generatedSignature);
            using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(keySecret)))
            {
                var hash = hmac.ComputeHash(generatedSignatureBytes);
                var generatedSignatureString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return generatedSignatureString == signature;
            }
        }

        [HttpGet("payment-history")]
        public IActionResult GetPaymentHistory()
        {
            var payments = _context.Payments.ToList();
            return Ok(payments);
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

    public class PaymentRequest
    {
        public int Amount { get; set; }
    }

    public class PaymentVerificationRequest
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }


}
