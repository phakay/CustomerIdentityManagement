using IdentityManagement.API.Core.Security.Models;

namespace IdentityManagement.API.Core.Infrastructure
{
    public interface ISmsService
    {
        Task SendAync(SecurityMessage message);
    }
}
