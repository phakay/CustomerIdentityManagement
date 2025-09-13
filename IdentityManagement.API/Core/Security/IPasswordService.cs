namespace IdentityManagement.API.Core.Security
{
    public interface IPasswordService
    {
        bool IsPasswordValid(string providedPassword, string passwordHash);
        string HashPassword(string password);
    }

}
