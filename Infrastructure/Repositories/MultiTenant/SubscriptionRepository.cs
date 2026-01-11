using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class SubscriptionRepository(IDbContextProvider dbContextProvider) : BaseRepository<SubscriptionEntity>(dbContextProvider), ISubscriptionRepository
    {
        public async Task<SubscriptionEntity?> GetActiveByTenantIdAsync(Guid tenantId)
        {
            return await FirstOrDefaultAsync(s => s.TenantId == tenantId && (s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial));
        }

        public async Task<SubscriptionEntity?> GetByTenantIdAsync(Guid tenantId)
        {
            return await FirstOrDefaultAsync(s => s.TenantId == tenantId);
        }
    }
}
