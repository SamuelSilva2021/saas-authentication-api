using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup
{
    public interface IGroupTypeService
    {
        Task<ResponseDTO<IEnumerable<GroupTypeDTO>>> GetAllAsync();
        Task<ResponseDTO<PagedResponseDTO<GroupTypeDTO>>> GetPagedAsync(int page, int limit);
        Task<ResponseDTO<GroupTypeDTO?>> GetByIdAsync(Guid id);
        Task<ResponseDTO<GroupTypeDTO>> CreateAsync(GroupTypeCreateDTO groupType);
        Task<ResponseDTO<GroupTypeDTO?>> UpdateAsync(Guid id, GroupTypeUpdateDTO groupType);
        Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
