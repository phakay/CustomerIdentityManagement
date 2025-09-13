using IdentityManagement.API.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityManagement.API.Config
{
    public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
    {
        private TokenConfigOptions _tokenConfig;
        private ITokenSigningProvider _tokenSigningProvider;

        public JwtBearerOptionsSetup(IOptions<TokenConfigOptions> tokenConfig, ITokenSigningProvider tokenSigningProvider)
        {
            _tokenConfig = tokenConfig.Value;
            _tokenSigningProvider = tokenSigningProvider;
        }

        public void Configure(JwtBearerOptions opt)
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _tokenConfig.Issuer,
                ValidAudience = _tokenConfig.Audience,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = _tokenSigningProvider.Key
            };
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }
}
