using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup
{
    public interface IAccessGroupService
    {
        Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetAllAsync();
        Task<ResponseDTO<AccessGroupDTO>> GetByIdAsync(Guid id);
        Task<ResponseDTO<AccessGroupDTO>> CreateAsync(CreateAccessGroupDTO dto);
        Task<ResponseDTO<AccessGroupDTO>> UpdateAsync(Guid id, UpdateAccessGroupDTO dto);
        Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
