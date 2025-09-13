using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace IdentityManagement.API.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class RefreshTokenDto
    {
        public string Username { get; set; }
        public string RefreshToken { get; set; }

    }

    public class CreateUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Length(9, 14)]
        public string PhoneNumber { get; set; }

        [Required]
        public int? LgaId { get; set; }

    }

    public class VerifyUserPhoneNumberRequest
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string Otp { get; set; }

    }

    public class InitiatePhonenumberVerificationRequest
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
    }
}
