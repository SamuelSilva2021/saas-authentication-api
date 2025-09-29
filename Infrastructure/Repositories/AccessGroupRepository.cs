using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;

namespace Authenticator.API.Infrastructure.Repositories
{
    public class AccessGroupRepository(
        IDbContextProvider dbContextProvider
        ) : BaseRepository<AccessGroupEntity>(dbContextProvider), IAccessGroupRepository
    {
        public async Task<IEnumerable<AccessGroupEntity>> GetAllAsyncByTenantId(Guid tenantId) =>
            await FindAsync(ag => ag.TenantId == tenantId);
    }
}
