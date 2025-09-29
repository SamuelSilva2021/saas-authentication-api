using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Infrastructure.Repositories.AccessGroup;

namespace Authenticator.API.Infrastructure.Repositories.Module
{
    /// <summary>
    /// Implementação específica do repositório para tipos de módulo
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class ModuleTypeRepository(
        IDbContextProvider dbContextProvider,
        ILogger<GroupTypeRepository> logger
        ) : BaseRepository<ModuleTypeEntity>(dbContextProvider), IModuleTypeRepository
    {
        private readonly ILogger<GroupTypeRepository> _logger = logger;

    }
}
