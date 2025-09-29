using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Authenticator.API.Infrastructure.Repositories.AccessGroup
{
    /// <summary>
    /// Implementação específica do repositório para tipos de grupo
    /// </summary>
    public class GroupTypeRepository(
            IDbContextProvider dbContextProvider,
            IMemoryCache cache,
            ILogger<GroupTypeRepository> logger
        ) : BaseRepository<GroupTypeEntity>(dbContextProvider), IGroupTypeRepository
    {
        private readonly ILogger<GroupTypeRepository> _logger = logger;
        private readonly IMemoryCache _cache = cache;

    }
}
