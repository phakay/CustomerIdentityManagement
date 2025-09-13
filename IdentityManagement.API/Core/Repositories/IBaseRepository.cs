using IdentityManagement.API.Core.Models;
using System.Linq.Expressions;

namespace IdentityManagement.API.Core.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> condition);

        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}
