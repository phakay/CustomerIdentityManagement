using IdentityManagement.API.Common;
using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Security.Models;

namespace IdentityManagement.API.Core.Security
{
    public interface IAuthenticationService
    {
        Task<Result<AccessToken>> CreateUserTokenAsync(string username, string password);

        Task<Result<AccessToken>> RefreshUserTokenAsync(string username, string refreshToken);
    }
}
