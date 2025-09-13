
using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Services;
using IdentityManagement.API.Dtos;
using IdentityManagement.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountsController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("/api/accounts/register")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Username = createUserDto.Username,
                PhoneNumber = createUserDto.PhoneNumber,
                LgaId = createUserDto.LgaId.HasValue ? createUserDto.LgaId.Value : default
            };

            var result = await _userService.CreateUserAsync(user, createUserDto.Password);
            if (!result.IsSuccessful)
                return BadRequest(result);

            var newUser = result.Value;

            var retUser = new UserDto
            {
                Id = newUser.Id,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Username = newUser.Username,
                PhoneNumber = newUser.PhoneNumber,
                IsPhoneNumberVerified = newUser.IsPhoneNumberVerified
            };

            return retUser;
        }

        [Authorize(Roles = "Administrator")]
        [Route("/api/admin/Users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            if (!result.IsSuccessful)
                return BadRequest(result);

            var retUsers = result.Value.Select(u => new UserDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToArray()
            });

            return retUsers.ToList();
        }


        [Route("/api/Users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            if (!result.IsSuccessful)
                return BadRequest(result);

            var retUsers = result.Value.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                PhoneNumber = u.PhoneNumber,
                IsPhoneNumberVerified = u.IsPhoneNumberVerified,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToArray(),
                Lga = u.Lga == null ? null : new LgaDto
                {
                    Id = u.Lga.Id,
                    Name = u.Lga.Name,
                    State = u.Lga.State.Name
                }
            });

            return retUsers.ToActionResult();
        }

        [Route("/api/Users/{id:int}")]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var result = await _userService.GetByUserIdAsync(id);
            if (!result.IsSuccessful)
                return BadRequest(result);

            var user = result.Value;

            var retUser = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                IsPhoneNumberVerified = user.IsPhoneNumberVerified,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray()
            };

            if (user.Lga != null)
            {
                var lga = user.Lga;
                retUser.Lga = new LgaDto
                {
                    Id = lga.Id,
                    Name = lga.Name,
                    State = lga.State.Name
                };
            }

            return retUser;
        }

        [Route("/api/accounts/verifyphonenumber")]
        [HttpPost]
        public async Task<ActionResult<ResponseDto<string>>> VerifyPhoneNumber([FromBody] VerifyUserPhoneNumberRequest request)
        {
            var result = await _userService.ProcessPhoneNumberVerification(request.UserEmail, request.Otp);
            if (!result.IsSuccessful)
                return BadRequest(result);

            return new ResponseDto<string> { Data = result.Value };
        }

        [Route("/api/accounts/initiatephonenumberverification")]
        [HttpPost]
        public async Task<ActionResult<ResponseDto<string>>> InitiatePhoneNumberVerification([FromBody] InitiatePhonenumberVerificationRequest request)
        {
            var result = await _userService.InitiatePhoneNumberVerification(request.UserEmail);
            if (!result.IsSuccessful)
                return BadRequest(result);

            return new ResponseDto<string> { Data = result.Value };
        }



        [Authorize]
        [HttpGet]
        [Route("/api/accounts/claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(x => new { Type = x.Type, Value = x.Value });
            return Ok(claims);
        }
    }
}
