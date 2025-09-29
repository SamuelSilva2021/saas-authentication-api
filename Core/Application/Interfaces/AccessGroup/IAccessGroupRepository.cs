using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;

namespace Authenticator.API.Core.Application.Interfaces.AccessGroup
{
    public interface IAccessGroupRepository : IBaseRepository<AccessGroupEntity>
    {
        Task<IEnumerable<AccessGroupEntity>> GetAllAsyncByTenantId(Guid tenantId);
    }
}
