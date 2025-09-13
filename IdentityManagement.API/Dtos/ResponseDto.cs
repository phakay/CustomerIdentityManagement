using System.Security.Principal;

namespace IdentityManagement.API.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string[] Roles { get; set; } = [];
        public LgaDto? Lga { get; set; }

    }

    public class LgaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }


    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LgaDto[] Lgas { get; set; } = [];
    }

    public class CreateUserResponseDto
    {
        public UserDto User { get; set; }
        public string Message { get; set; }
    }

    public class ResponseDto<T>
    {
        public T Data { get; set; }
    }
}
