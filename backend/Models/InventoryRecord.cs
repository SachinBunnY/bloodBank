// Models/InventoryRecord.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BloodBank.Backend.Models
{
    public class InventoryRecord
    {
        [Key]
        public int Id { get; set; }
        public string BloodGroup { get; set; }
        public string InventoryType { get; set; }
        public int Quantity { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
