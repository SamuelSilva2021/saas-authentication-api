using Authenticator.API.Core.Application.Interfaces.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessGroup
{
    public class GroupTypeService(
        IMapper mapper,
        IGroupTypeRepository groupTypeRepository
        ) : IGroupTypeService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGroupTypeRepository _groupTypeRepository = groupTypeRepository;

        /// <summary>
        /// Cria um novo tipo de grupo
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public async Task<ApiResponse<GroupTypeDTO>> CreateAsync(GroupTypeCreateDTO groupType)
        {
            var existingGroupType = await _groupTypeRepository.FindAsync(gt => gt.Name == groupType.Name);
            if (existingGroupType is not null && existingGroupType.Any())
                return new ApiResponse<GroupTypeDTO> { Success = false, Message = "Já existe um tipo de grupo com esse nome." };

            var groupTypeEntity = _mapper.Map<GroupTypeEntity>(groupType);
            try
            {
                await _groupTypeRepository.AddAsync(groupTypeEntity);
                return _mapper.Map<ApiResponse<GroupTypeDTO>>(groupTypeEntity);
            }
            catch (Exception ex)
            {
                return new ApiResponse<GroupTypeDTO> { Success = false, Message = $"Erro ao criar o tipo de grupo: {ex.Message}" };
            }
            
            
        }

        /// <summary>
        /// Deleta um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var existingGroupType = await _groupTypeRepository.GetByIdAsync(id);
            if (existingGroupType == null)
                return new ApiResponse<bool> { Success = false, Message = "Tipo de grupo não encontrado", Data = false };
            try
            {
                await _groupTypeRepository.DeleteAsync(existingGroupType);
                return new ApiResponse<bool> { Success = true, Message = "Tipo de grupo deletado com sucesso", Data = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Erro ao deletar o tipo de grupo: {ex.Message}", Data = false };
            }
        }

        /// <summary>
        /// Recupera todos os tipos de grupo
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<IEnumerable<GroupTypeDTO>>> GetAllAsync()
        {
           var groupTypeEntities = await _groupTypeRepository.GetAllAsync();
            var groupTypes = _mapper.Map<IEnumerable<GroupTypeDTO>>(groupTypeEntities);

            return new ApiResponse<IEnumerable<GroupTypeDTO>>
            {
                Success = true,
                Message = groupTypes.Any() ? string.Empty : "Nenhum tipo de grupo encontrado",
                Data = groupTypes
            };
        }

        /// <summary>
        /// Recupera um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<GroupTypeDTO?>> GetByIdAsync(Guid id)
        {
            var groupTypeEntity = await _groupTypeRepository.GetByIdAsync(id);
            if (groupTypeEntity == null)
                return new ApiResponse<GroupTypeDTO?> { Success = false, Message = "Tipo de grupo não encontrado", Data = null };

            return new ApiResponse<GroupTypeDTO?> { 
                Success = true, 
                Message = string.Empty, 
                Data = _mapper.Map<GroupTypeDTO>(groupTypeEntity) 
            };
        }

        /// <summary>
        /// Atualiza um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<GroupTypeDTO?>> UpdateAsync(Guid id, GroupTypeUpdateDTO groupType)
        {
            try
            {
                var existingGroupType = await _groupTypeRepository.GetByIdAsync(id);

                if (existingGroupType == null)
                    return new ApiResponse<GroupTypeDTO?> { Success = false, Message = "Tipo de grupo não encontrado", Data = null };

                var duplicateGroupType = await _groupTypeRepository.FindAsync(gt => gt.Name == groupType.Name && gt.Id != id);

                if (duplicateGroupType is not null && duplicateGroupType.Any())
                    return new ApiResponse<GroupTypeDTO?> { Success = false, Message = "Já existe um tipo de grupo com esse nome.", Data = null };

                _mapper.Map(groupType, existingGroupType);
                await _groupTypeRepository.UpdateAsync(existingGroupType);

                return new ApiResponse<GroupTypeDTO?>
                {
                    Success = true,
                    Message = "Tipo de grupo atualizado com sucesso",
                    Data = _mapper.Map<GroupTypeDTO>(existingGroupType)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GroupTypeDTO?> { Success = false, Message = $"Erro ao atualizar o tipo de grupo: {ex.Message}", Data = null };
            }
            
        }
    }
}
