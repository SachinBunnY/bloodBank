using System.Threading.Tasks;
using BloodBank.Backend.Models;

namespace BloodBank.Backend.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetCurrentUserAsync(int userId);
        Task<User> Register(User user, string password);
        Task<string> Login(string email, string password);
        Task<User> GetCurrentUser(int userId);
    }
}