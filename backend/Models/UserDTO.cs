// File: Models/UserDTO.cs

namespace BloodBank.Backend.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string OrganisationName { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}
