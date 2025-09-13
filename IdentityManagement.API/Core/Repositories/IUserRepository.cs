using IdentityManagement.API.Core.Models;

namespace IdentityManagement.API.Core.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> FindByUsernameAsync(string username);
    }
}
