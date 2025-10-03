using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.Permissions;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Permissions
{
    public class PermissionRepository(
        IDbContextProvider dbContextProvider,
        ILogger<PermissionRepository> logger
            ) : BaseRepository<PermissionEntity>(dbContextProvider), IPermissionRepository
    {
        private readonly ILogger<PermissionRepository> _logger = logger;
    }
}
