using IdentityManagement.API.Common;
using IdentityManagement.API.Config;
using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;
using IdentityManagement.API.Core.Security;
using IdentityManagement.API.Core.Security.Models;
using Microsoft.Extensions.Options;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityManagement.API.Security
{
    public class AuthenticationServiceWithoutCache : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordService _passwordService;
        private readonly ITokenSigningProvider _tokenSigningProvider;
        private readonly ISecurityProvider _securityProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenConfigOptions _tokenConfigOptions;

        public AuthenticationServiceWithoutCache(
            IUserRepository userRepo,
            IPasswordService passwordService,
            IOptions<TokenConfigOptions> tokenConfig,
            ITokenSigningProvider tokenSigningProvider,
            ISecurityProvider securityProvider,
            IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _passwordService = passwordService;
            _tokenConfigOptions = tokenConfig.Value;
            _tokenSigningProvider = tokenSigningProvider;
            _securityProvider = securityProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AccessToken>> CreateUserTokenAsync(string username, string password)
        {
            var user = await _userRepo.FindByUsernameAsync(username);
            if (user == null) return Result<AccessToken>.Failure("user not found");

            var isPasswordValid = _passwordService.IsPasswordValid(password, user.PasswordHash);
            if (!isPasswordValid) return Result<AccessToken>.Failure("invalid credential");

            _securityProvider.SetSecurityStampAsync(user);

            AccessToken accessToken = BuildAccessToken(user);

            await _unitOfWork.SaveChangesAsync();

            return Result<AccessToken>.Success(accessToken);
        }

        public async Task<Result<AccessToken>> RefreshUserTokenAsync(string username, string refreshToken)
        {
            var user = await _userRepo.FindByUsernameAsync(username);
            if (user == null) return Result<AccessToken>.Failure("user not found");

            if (!IsRefreshTokenValid(refreshToken, user))
            {
                return Result<AccessToken>.Failure("invalid refresh token");
            }

            _securityProvider.SetSecurityStampAsync(user);

            var accessToken = BuildAccessToken(user);

            await _unitOfWork.SaveChangesAsync();

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
            var timestamp = DateTime.UtcNow.Ticks / TimeSpan.FromMinutes(_tokenConfigOptions.RefreshTokenDurationInMins).Ticks;
            var data = $"{user.SecurityStamp}:{timestamp}";
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(data));
            var refreshToken = Convert.ToBase64String(hash);

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

        private bool IsRefreshTokenValid(string refreshToken, User user)
        {
            if (!Base64.IsValid(refreshToken.AsSpan()))
                return false;

            var dt = DateTime.UtcNow.Ticks / TimeSpan.FromMinutes(_tokenConfigOptions.RefreshTokenDurationInMins).Ticks;

            var refreshTokenBytes = Convert.FromBase64String(refreshToken);

            for (var offset = -1; offset <= 1; offset++) // cater for edge cases due to approx.
            {
                var timestamp = dt + offset;
                var data = $"{user.SecurityStamp}:{timestamp}";
                var hash = MD5.HashData(Encoding.UTF8.GetBytes(data));


                if (refreshTokenBytes.SequenceEqual(hash))
                    return true;
            }

            return false;
        }
    }
}
