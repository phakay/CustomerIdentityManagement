using IdentityManagement.API.Common;
using IdentityManagement.API.Config;
using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;
using IdentityManagement.API.Core.Security;
using IdentityManagement.API.Core.Security.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityManagement.API.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordService _passwordService;
        private readonly IMemoryCache _cache;
        private readonly ITokenSigningProvider _tokenSigningProvider;
        private readonly TokenConfigOptions _tokenConfigOptions;
        private const string CacheGroup = "auth-group";

        public AuthenticationService(
            IUserRepository userRepo,
            IMemoryCache cache,
            IPasswordService passwordService,
            IOptions<TokenConfigOptions> tokenConfig, 
            ITokenSigningProvider tokenSigningProvider)
        {
            _userRepo = userRepo;
            _cache = cache;
            _passwordService = passwordService;
            _tokenConfigOptions = tokenConfig.Value;
            _tokenSigningProvider = tokenSigningProvider;
        }

        public async Task<Result<AccessToken>> CreateUserTokenAsync(string username, string password)
        {
            var user = await _userRepo.FindByUsernameAsync(username);
            if (user == null) return Result<AccessToken>.Failure("user not found");

            var isPasswordValid = _passwordService.IsPasswordValid(password, user.PasswordHash);
            if (!isPasswordValid) return Result<AccessToken>.Failure("invalid credential");

            AccessToken accessToken = BuildAccessToken(user);

            return Result<AccessToken>.Success(accessToken);
        }

        public async Task<Result<AccessToken>> RefreshUserTokenAsync(string username, string refreshToken)
        {
            var user = await _userRepo.FindByUsernameAsync(username);
            if (user == null) return Result<AccessToken>.Failure("user not found");

            var key = GetKey(username);
            if (!IsRefreshTokenValid(refreshToken, key))
            {
                return Result<AccessToken>.Failure("invalid refresh token");
            }

            var accessToken = BuildAccessToken(user);

            return Result<AccessToken>.Success(accessToken);
        }

        private AccessToken BuildAccessToken(User user)
        {
            string bearerToken = BuildBearerToken(user);
            string refreshToken = BuildRefreshToken(user);

            var accessToken = new AccessToken { BearerToken = bearerToken, RefreshToken = refreshToken };
            return accessToken;
        }

        private string BuildRefreshToken(User user)
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var key = GetKey(user.Username);

            _cache.Set(key, refreshToken, TimeSpan.FromMinutes(_tokenConfigOptions.RefreshTokenDurationInMins));
            return refreshToken;
        }

        private string BuildBearerToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var roleClaims = user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.Name));
            if (roleClaims.Any())
                claims.AddRange(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _tokenConfigOptions.Issuer,
                audience: _tokenConfigOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_tokenConfigOptions.BearerTokenDurationInMins),
                signingCredentials: _tokenSigningProvider.SigningCredentials
             );

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.WriteToken(token);
            return jwtToken;
        }

        private bool IsRefreshTokenValid(string refreshToken, string key)
        {
            return _cache.TryGetValue(key, out string? value) && value != null && string.Equals(value, refreshToken, StringComparison.Ordinal);
        }

        private static string GetKey(string name) => $"{CacheGroup}-{name}";
    }
}
