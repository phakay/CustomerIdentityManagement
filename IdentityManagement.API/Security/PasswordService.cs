using IdentityManagement.API.Core.Security;
using System.Security.Cryptography;
using System.Text;

namespace IdentityManagement.API.Security
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hashBytes);
        }

        public bool IsPasswordValid(string providedPassword, string passwordHash)
        {
            var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(providedPassword));

            return string.Equals(passwordHash, Convert.ToBase64String(hashBytes), StringComparison.Ordinal);
        }
    }
}
