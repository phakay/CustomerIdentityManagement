namespace IdentityManagement.API.Core.Security.Models
{
    public class AccessToken
    {
        public string BearerToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
