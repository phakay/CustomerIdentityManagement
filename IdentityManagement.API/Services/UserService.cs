using IdentityManagement.API.Common;
using IdentityManagement.API.Core.Infrastructure;
using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;
using IdentityManagement.API.Core.Security;
using IdentityManagement.API.Core.Security.Models;
using IdentityManagement.API.Core.Services;
using IdentityManagement.API.Extensions;

namespace IdentityManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILgaRepository _lgaRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ISecurityProvider _securityProvider;
        private readonly ISmsService _smsService;

        public UserService(
            IUserRepository userRepo,
            IPasswordService passwordService,
            ILgaRepository lgaRepo,
            ISecurityProvider securityProvider,
            IUnitOfWork unitOfWork,
            ISmsService smsService)
        {
            _userRepo = userRepo;
            _passwordService = passwordService;
            _lgaRepo = lgaRepo;
            _securityProvider = securityProvider;
            _unitOfWork = unitOfWork;
            _smsService = smsService;
        }

        public async Task<Result<User>> CreateUserAsync(User user, string password)
        {
            var userExists = await _userRepo.ExistsAsync(x => x.Username == user.Username);

            if (userExists) return Result<User>.Failure("username exists");

            var lgaExists = await _lgaRepo.ExistsAsync(x => user.LgaId.HasValue && x.Id == user.LgaId.Value);

            if (!lgaExists) return Result<User>.Failure("lga is invalid.");

            user.PasswordHash = _passwordService.HashPassword(password);

            _securityProvider.SetSecurityStampAsync(user);

            user.IsPhoneNumberVerified = false;

            await _userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await SendUserOtpAsync(user);

            return user;
        }

        public async Task<Result<string>> ProcessPhoneNumberVerification(string userEmail, string otp)
        {
            var user = await _userRepo.FindByUsernameAsync(userEmail);

            if (user == null) return Result<string>.Failure("user email does not exist");

            if (user.IsPhoneNumberVerified) 
                return Result<string>.Success("user phonenumber already verified");

            var isOtpValid = _securityProvider.ValidateOtp(user, otp);
            if (!isOtpValid) return Result<string>.Failure("invalid otp");

            _securityProvider.SetSecurityStampAsync(user);

            user.IsPhoneNumberVerified = true;

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success("user phonenumber verified.");
        }


        public async Task<Result<string>> InitiatePhoneNumberVerification(string userEmail)
        {
            var user = await _userRepo.FindByUsernameAsync(userEmail);

            if (user == null) return Result<string>.Failure("user email does not exist");

            await SendUserOtpAsync(user);

            user.IsPhoneNumberVerified = false;

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success("otp sent");
        }


        public async Task<Result<User>> GetByUserIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return Result<User>.Failure("user not found");

            return user;
        }

        public async Task<Result<IEnumerable<User>>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.ToResult();
        }

        private async Task SendUserOtpAsync(User user)
        {
            string code = _securityProvider.GenerateOtp(user);

            var message = new SecurityMessage
            {
                To = user.PhoneNumber,
                Subject = "Otp Confirmation",
                Body = $"Your otp code is {code}"
            };

            await _smsService.SendAync(message);
        }
    }
}
