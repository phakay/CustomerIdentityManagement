using IdentityManagement.API.Core.Models;

namespace IdentityManagement.API.Core.Security
{
    public interface ISecurityProvider
    {
        string GenerateOtp(User user);
        void SetSecurityStampAsync(User user);
        bool ValidateOtp(User user, string code);
    }
}
