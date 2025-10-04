using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup
{
    /// <summary>
    /// Interface para o servi�o de tipos de grupo
    /// </summary>
    public interface IGroupTypeService
    {
        /// <summary>
        /// Obt�m todos os tipos de grupo
        /// </summary>
        /// <returns></returns>
        Task<ResponseDTO<IEnumerable<GroupTypeDTO>>> GetAllAsync();
        /// <summary>
        /// Obt�m tipos de grupo paginados
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<ResponseDTO<PagedResponseDTO<GroupTypeDTO>>> GetPagedAsync(int page, int limit);
        /// <summary>
        /// Obt�m um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<GroupTypeDTO?>> GetByIdAsync(Guid id);
        /// <summary>
        /// Cria um novo tipo de grupo
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        Task<ResponseDTO<GroupTypeDTO>> CreateAsync(GroupTypeCreateDTO groupType);
        /// <summary>
        /// Atualiza um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        Task<ResponseDTO<GroupTypeDTO?>> UpdateAsync(Guid id, GroupTypeUpdateDTO groupType);
        /// <summary>
        /// Deleta um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
