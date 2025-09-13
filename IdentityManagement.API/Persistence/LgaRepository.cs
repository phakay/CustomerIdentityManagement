using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;

namespace IdentityManagement.API.Persistence
{
    public class LgaRepository : BaseRepository<Lga>, ILgaRepository
    {
        public LgaRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
