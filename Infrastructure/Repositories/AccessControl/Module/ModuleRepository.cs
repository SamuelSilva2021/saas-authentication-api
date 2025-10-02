using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Infrastructure.Repositories.AccessControl.AccessGroup;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Module
{
    /// <summary>
    /// Implementação específica do repositório para tipos de módulo
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class ModuleRepository(
        IDbContextProvider dbContextProvider,
        ILogger<GroupTypeRepository> logger
        ) : BaseRepository<ModuleEntity>(dbContextProvider), IModuleRepository
    {

    }
}
