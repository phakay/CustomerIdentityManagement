using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IdentityManagement.API.Persistence
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        { }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            var user = await _dbContext.Users.Include(x => x.UserRoles)
                    .ThenInclude(y => y.Role)
                    .FirstOrDefaultAsync(z => z.Username == username);

            return user;
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            var user = await _dbContext.Users
                            .Include(x => x.Lga).ThenInclude(y => y.State)
                            .Include(x => x.UserRoles).ThenInclude(y => y.Role)
                            .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _dbContext.Users
                    .Include(x => x.Lga).ThenInclude(y => y.State)
                    .Include(x => x.UserRoles)
                    .ThenInclude(y => y.Role)
                    .AsNoTracking()
                    .ToListAsync();

            return users;
        }
    }
}
