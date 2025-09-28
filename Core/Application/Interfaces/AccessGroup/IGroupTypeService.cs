using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessGroup
{
    public interface IGroupTypeService
    {
        Task <ApiResponse<IEnumerable<GroupTypeDTO>>> GetAllAsync();
        Task<ApiResponse<GroupTypeDTO?>> GetByIdAsync(Guid id);
        Task<ApiResponse<GroupTypeDTO>> CreateAsync(GroupTypeCreateDTO groupType);
        Task<ApiResponse<GroupTypeDTO?>> UpdateAsync(Guid id, GroupTypeUpdateDTO groupType);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
