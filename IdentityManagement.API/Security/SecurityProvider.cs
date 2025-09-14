using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Security;
using System.Security.Cryptography;
using System.Text;

namespace IdentityManagement.API.Security
{
    public class SecurityProvider : ISecurityProvider
    {
        public void SetSecurityStampAsync(User user)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        public string GenerateOtp(User user)
        {
            var timestamp  = DateTime.UtcNow.Ticks / TimeSpan.FromMinutes(2).Ticks; // expires in 2mins
            var data = $"{user.SecurityStamp}:{timestamp}";
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(data));
            var code = (BitConverter.ToUInt32(hash, 0) % 10000).ToString("D4");
            return code;
        }

        public bool ValidateOtp(User user, string code)
        {
            var dt = DateTime.UtcNow.Ticks / TimeSpan.FromMinutes(2).Ticks;
            for (var offset = -1; offset <= 1; offset++) // cater for edge cases due to approx.
            {
                var timestamp = dt + offset;
                var data = $"{user.SecurityStamp}:{timestamp}";
                var hash = MD5.HashData(Encoding.UTF8.GetBytes(data));
                var generatedCode = (BitConverter.ToUInt32(hash, 0) % 10000).ToString("D4");

                if (string.Equals(code, generatedCode, StringComparison.Ordinal))
                    return true;
            }

            return false;

        }
    }
}
