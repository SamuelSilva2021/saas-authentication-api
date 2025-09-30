using Authenticator.API.Core.Domain.MultiTenant.Tenant;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ITenantRepository : IBaseRepository<TenantEntity>
    {
        Task<TenantEntity?> GetByDocumentAsync(string document);
        Task<bool> ExistingSlug(string slug);
    }
}
