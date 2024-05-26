using System.ComponentModel.DataAnnotations;



namespace BloodBank.Backend.Models
{

    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentId { get; set; } // Make PaymentId nullable
        public string? Signature { get; set; } // Make Signature nullable
        public string Status { get; set; }
    }
}

