using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessGroup
{
    public interface IAccessGroupService
    {
        Task<ApiResponse<IEnumerable<AccessGroupDTO>>> GetAllAsync();
        Task<ApiResponse<AccessGroupDTO>> GetByIdAsync(Guid id);
        Task<ApiResponse<AccessGroupDTO>> CreateAsync(CreateAccessGroupDTO dto);
        Task<ApiResponse<AccessGroupDTO>> UpdateAsync(Guid id, UpdateAccessGroupDTO dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
