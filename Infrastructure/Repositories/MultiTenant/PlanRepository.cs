using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.MultiTenant.Plan;

namespace Authenticator.API.Infrastructure.Repositories.MultiTenant
{
    public class PlanRepository(IDbContextProvider dbContextProvider) : BaseRepository<PlanEntity>(dbContextProvider), IPlanRepository
    {
        public async Task<PlanEntity?> GetDefaultPlanAsync()
        {
            return await FirstOrDefaultAsync(p => true);
        }
    }
}
