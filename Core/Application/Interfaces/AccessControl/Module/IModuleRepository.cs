using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Modules.Entities;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Module
{
    public interface IModuleRepository : IBaseRepository<ModuleEntity>
    {
        Task<UserPermissionsDTO> GetModulesByUserAsync(Guid userId);
    }
}
