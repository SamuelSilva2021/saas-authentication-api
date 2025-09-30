using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.AccessGroup
{
    public class AccessGroupRepository(
        IDbContextProvider dbContextProvider
        ) : BaseRepository<AccessGroupEntity>(dbContextProvider), IAccessGroupRepository
    {
        public async Task<IEnumerable<AccessGroupEntity>> GetAllAsyncByTenantId(Guid tenantId) =>
            await FindAsync(ag => ag.TenantId == tenantId);
    }
}
