using Authenticator.API.Core.Application.Interfaces.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessGroup
{
    /// <summary>
    /// Serviço para gerenciamento de grupos de acesso
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="accessGroupRepository"></param>
    /// <param name="mapper"></param>
    public class AccessGroupService(ILogger<AccessGroupService> logger, IAccessGroupRepository accessGroupRepository, IMapper mapper) : IAccessGroupService
    {
        private readonly ILogger<AccessGroupService> _logger = logger;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Cria um novo grupo de acesso
        /// </summary>
        /// <param name="createAccessGroupDTO"></param>
        /// <returns></returns>
        public async Task<ApiResponse<AccessGroupDTO>> CreateAsync(CreateAccessGroupDTO createAccessGroupDTO)
        {
            try
            {
                var entity = _mapper.Map<AccessGroupEntity>(createAccessGroupDTO);
                var createdEntity = await _accessGroupRepository.AddAsync(entity);

                var dto = _mapper.Map<AccessGroupDTO>(createdEntity);

                return ApiResponse<AccessGroupDTO>.SuccessResult(dto, "Access group created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar o grupo de acesso");
                return ApiResponse<AccessGroupDTO>.ErrorResult($"Ocorreu um erro ao criar o grupo de acesso.");
            }
        }

        /// <summary>
        /// Deleta um grupo de acesso por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _accessGroupRepository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<bool>.ErrorResult("Grupo de accesso não encontrado");

                await _accessGroupRepository.DeleteAsync(entity);
                return ApiResponse<bool>.SuccessResult(true, "Grupo de acesso excluído com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar grupo de acesso.");
                return ApiResponse<bool>.ErrorResult($"Ocorreu um erro ao deletar o grupo de acesso.");
            }
        }

        /// <summary>
        /// Recupera todos os grupos de acesso
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<IEnumerable<AccessGroupDTO>>> GetAllAsync()
        {
            try
            {
                var entities = await _accessGroupRepository.GetAllAsync();
                if (entities == null || !entities.Any())
                    return ApiResponse<IEnumerable<AccessGroupDTO>>.ErrorResult("Nenhum grupo de acesso encontrado.");

                var dto = _mapper.Map<IEnumerable<AccessGroupDTO>>(entities);
                return ApiResponse<IEnumerable<AccessGroupDTO>>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar grupos de acesso");
                return ApiResponse<IEnumerable<AccessGroupDTO>>.ErrorResult("Ocorreu um erro ao buscar grupos de acesso.");
            }
            
        }

        public async Task<ApiResponse<AccessGroupDTO>> GetByIdAsync(Guid id)
        {
            var entity = await _accessGroupRepository.GetByIdAsync(id);
            if (entity == null)
                return ApiResponse<AccessGroupDTO>.ErrorResult("Grupo de accesso não encontrado");
            return ApiResponse<AccessGroupDTO>.SuccessResult(_mapper.Map<AccessGroupDTO>(entity));

        }

        /// <summary>
        /// Atualiza um grupo de acesso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<AccessGroupDTO>> UpdateAsync(Guid id, UpdateAccessGroupDTO dto)
        {
            try
            {
                var entity = await _accessGroupRepository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<AccessGroupDTO>.ErrorResult("Grupo de accesso não encontrado");
                var updatedEntity = _mapper.Map(dto, entity);
                await _accessGroupRepository.UpdateAsync(updatedEntity);
                return ApiResponse<AccessGroupDTO>.SuccessResult(_mapper.Map<AccessGroupDTO>(updatedEntity), "Grupo de acesso atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o grupo de acesso.");
                return ApiResponse<AccessGroupDTO>.ErrorResult("Ocorreu um erro ao atualizar o grupo de acesso.");
            }
        }
    }
}
