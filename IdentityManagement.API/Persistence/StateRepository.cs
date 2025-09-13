using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagement.API.Persistence
{
    public class StateRepository : BaseRepository<State>, IStateRepository
    {
        public StateRepository(AppDbContext dbContext) : base(dbContext)
        { }

        public override async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _dbContext.States.AsNoTracking().ToListAsync();
        }

        public override async Task<State?> GetByIdAsync(int id)
        {
            return await _dbContext.States.Include(x => x.Lgas).FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
