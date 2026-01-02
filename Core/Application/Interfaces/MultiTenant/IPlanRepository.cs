using Authenticator.API.Core.Domain.MultiTenant.Plan;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    public interface IPlanRepository : IBaseRepository<PlanEntity>
    {
        Task<PlanEntity?> GetDefaultPlanAsync();
    }
}
