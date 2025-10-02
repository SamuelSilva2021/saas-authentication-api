using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Module
{
    public interface IModuleService
    {
        Task<ResponseDTO<IEnumerable<ModuleDTO>>> GetAllModuleAsync();
        Task<ResponseDTO<ModuleDTO>> GetModuleByIdAsync(Guid id);
        Task<ResponseDTO<ModuleDTO>> AddModuleAsync(ModuleCreateDTO moduleType);
        Task<ResponseDTO<ModuleDTO>> UpdateModuleAsync(Guid id, ModuleUpdateDTO moduleType);
        Task<ResponseDTO<bool>> DeleteModuleAsync(Guid id);
    }
}
