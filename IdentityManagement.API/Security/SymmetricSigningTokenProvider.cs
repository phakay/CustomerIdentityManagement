using IdentityManagement.API.Core.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityManagement.API.Security
{
    public class SymmetricSigningTokenProvider : ITokenSigningProvider
    {
        private SigningCredentials _signingCreds;


        public SymmetricSigningTokenProvider(string secret)
        {
            var sk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            _signingCreds = new SigningCredentials(sk, SecurityAlgorithms.HmacSha256Signature);
        }

        public SecurityKey Key => _signingCreds.Key;
        public SigningCredentials SigningCredentials => _signingCreds;
    }
}
