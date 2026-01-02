using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class SubscriptionRepository(IDbContextProvider dbContextProvider) : BaseRepository<SubscriptionEntity>(dbContextProvider), ISubscriptionRepository
    {
        public async Task<SubscriptionEntity?> GetActiveByTenantIdAsync(Guid tenantId)
        {
            return await FirstOrDefaultAsync(s => s.TenantId == tenantId && (s.Status == "active" || s.Status == "trialing"));
        }

        public async Task<SubscriptionEntity?> GetByTenantIdAsync(Guid tenantId)
        {
            return await FirstOrDefaultAsync(s => s.TenantId == tenantId);
        }
    }
}
