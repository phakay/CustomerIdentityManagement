using IdentityManagement.API.Common;
using IdentityManagement.API.Core.Models;

namespace IdentityManagement.API.Core.Services
{
    public interface IUserService
    {
        Task<Result<User>> GetByUserIdAsync(int id);
        Task<Result<User>> CreateUserAsync(User user, string password);
        Task<Result<IEnumerable<User>>> GetAllUsersAsync();
        Task<Result<string>> ProcessPhoneNumberVerification(string userEmail, string otp);
        Task<Result<string>> InitiatePhoneNumberVerification(string userEmail);
    }
}
