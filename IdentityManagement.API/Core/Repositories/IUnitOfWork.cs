namespace IdentityManagement.API.Core.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
