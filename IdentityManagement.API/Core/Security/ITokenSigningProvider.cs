using Microsoft.IdentityModel.Tokens;

namespace IdentityManagement.API.Core.Security
{
    public interface ITokenSigningProvider
    {
        SecurityKey Key { get; }

        SigningCredentials SigningCredentials { get; }
    }
}
