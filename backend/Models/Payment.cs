using System.ComponentModel.DataAnnotations;



namespace BloodBank.Backend.Models
{

    // public class Payment
    // {
    //     public int Id { get; set; }
    //     public decimal Amount { get; set; }
    //     public string OrderId { get; set; }
    //     public DateTime PaymentDate { get; set; }
    //     public string? PaymentId { get; set; } // Make PaymentId nullable
    //     public string? Signature { get; set; } // Make Signature nullable
    //     public string Status { get; set; }
    // }
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? OrderId { get; set; }
        public string BloodGroup { get; set; }
        public string BloodQuantity { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Signature { get; set; }
        public string? PaymentId { get; set; }
        public string? BankRRN { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string BloodOwner { get; set; }

        public string Status { get; set; }
    }
}

