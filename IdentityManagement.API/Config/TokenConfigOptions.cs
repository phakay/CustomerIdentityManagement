namespace IdentityManagement.API.Config
{
    public class TokenConfigOptions
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int BearerTokenDurationInMins { get; set; }
        public int RefreshTokenDurationInMins { get; set; }
    }
}
