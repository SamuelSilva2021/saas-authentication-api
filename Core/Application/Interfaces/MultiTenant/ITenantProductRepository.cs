using Authenticator.API.Core.Domain.MultiTenant.TenantProduct;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ITenantProductRepository : IBaseRepository<TenantProductEntity>
    {
        Task<TenantProductEntity?> GetDefaultProductAsync();
    }
}
