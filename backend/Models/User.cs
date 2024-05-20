using System.ComponentModel.DataAnnotations;



namespace BloodBank.Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string OrganisationName { get; set; }
        public string HospitalName { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

}