using IdentityManagement.API.Core.Security;
using IdentityManagement.API.Core.Security.Models;
using IdentityManagement.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [Route("/api/auth/login")]
        [HttpPost]
        public async Task<ActionResult<AccessToken>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.CreateUserTokenAsync(loginDto.Username, loginDto.Password);
            if (!result.IsSuccessful)
                return BadRequest(result);

            return result.Value;
        }

        [Route("/api/auth/refreshtoken")]
        [HttpPost]
        public async Task<ActionResult<AccessToken>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshUserTokenAsync(refreshTokenDto.Username, refreshTokenDto.RefreshToken);
            if (!result.IsSuccessful)
                return BadRequest(result);

            return result.Value;
        }
    }
}
