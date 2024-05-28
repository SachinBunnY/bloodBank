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
using Payment = BloodBank.Backend.Models.Payment;

namespace BloodBank.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger, Data.ApplicationDbContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        // RAZOR PAYMENT INTEGRATION START FROM HERE
        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] PaymentRequest request)
        {
            try
            {
                _logger.LogError("INSIDE CREATE ORDER");
                var client = new RazorpayClient(_config["Razorpay:KeyId"], _config["Razorpay:KeySecret"]);
                _logger.LogError("Client ID:{client}", client);

                var options = new Dictionary<string, object>
                {
                    { "amount", request.Amount * 100 },
                    { "currency", "INR" },
                    { "receipt",  Guid.NewGuid().ToString() },
                    { "payment_capture", 1 }
                };
                var order = client.Order.Create(options);
                _logger.LogError("Order ID:{order}", order);


                var payment = new Payment
                {
                    OrderId = order["id"].ToString(),
                    Amount = request.Amount,
                    BloodGroup = request.BloodGroup,
                    BloodQuantity = request.Quantity,
                    Contact = request.Phone,
                    Email = request.Email,
                    PaymentDate = DateTime.UtcNow,
                    BloodOwner = request.Author,
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
            _logger.LogError("Inside payment history.");
            var payments = _context.Payments.ToList();
            _logger.LogError("All payments:{payments}", payments);
            return Ok(payments);
        }

    }

    public class PaymentRequest
    {
        public int Amount { get; set; }
        public string Author { get; set; }
        public string BloodGroup { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Quantity { get; set; }

    }

    public class PaymentVerificationRequest
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }

}