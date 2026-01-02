using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface ISubscriptionRepository : IBaseRepository<SubscriptionEntity>
    {
        Task<SubscriptionEntity?> GetActiveByTenantIdAsync(Guid tenantId);
        Task<SubscriptionEntity?> GetByTenantIdAsync(Guid tenantId);
    }
}
