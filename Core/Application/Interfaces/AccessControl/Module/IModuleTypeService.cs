using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Module
{
    public interface IModuleTypeService
    {
        Task<ResponseDTO<IEnumerable<ModuleTypeDTO>>> GetAllModuleTypesAsync();
        Task<ResponseDTO<ModuleTypeDTO>> GetModuleTypeByIdAsync(Guid id);
        Task<ResponseDTO<ModuleTypeDTO>> AddModuleTypeAsync(ModuleTypeCreateDTO moduleType);
        Task<ResponseDTO<ModuleTypeDTO>> UpdateModuleTypeAsync(Guid id, ModuleTypeUpdateDTO moduleType);
        Task<ResponseDTO<bool>> DeleteModuleTypeAsync(Guid id);
    }
}
