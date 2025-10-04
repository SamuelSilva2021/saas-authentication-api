using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.PermissionOperations
{
    /// <summary>
    /// Serviço para gerenciar relações Permissão-Operação
    /// </summary>
    /// <param name="permissionOperationRepository"></param>
    /// <param name="permissionRepository"></param>
    /// <param name="operationRepository"></param>
    /// <param name="mapper"></param>
    public class PermissionOperationService(
        IPermissionOperationRepository permissionOperationRepository,
        IPermissionRepository permissionRepository,
        IOperationRepository operationRepository,
        IMapper mapper) : IPermissionOperationService
    {
        private readonly IPermissionOperationRepository _permissionOperationRepository = permissionOperationRepository;
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IOperationRepository _operationRepository = operationRepository;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Obtém todas as relações Permissão-Operação
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetAllPermissionOperationsAsync()
        {
            try
            {
                var entities = await _permissionOperationRepository.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<PermissionOperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém relações por ID da permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                var entities = await _permissionOperationRepository.GetByPermissionIdAsync(permissionId);
                var dtos = _mapper.Map<IEnumerable<PermissionOperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém relações por ID da operação
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> GetByOperationIdAsync(Guid operationId)
        {
            try
            {
                var entities = await _permissionOperationRepository.GetByOperationIdAsync(operationId);
                var dtos = _mapper.Map<IEnumerable<PermissionOperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém uma relação específica entre permissão e operação
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionOperationDTO>> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId)
        {
            try
            {
                var entity = await _permissionOperationRepository.GetByPermissionAndOperationAsync(permissionId, operationId);
                
                if (entity == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "Relação Permissão-Operação não encontrada" })
                        .WithCode(404)
                        .Build();

                var dto = _mapper.Map<PermissionOperationDTO>(entity);
                return ResponseBuilder<PermissionOperationDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionOperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Cria uma nova relação Permissão-Operação
        /// </summary>
        /// <param name="permissionOperation"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionOperationDTO>> CreatePermissionOperationAsync(PermissionOperationCreateDTO permissionOperation)
        {
            try
            {
                // Validar se a permissão existe
                var permission = await _permissionRepository.GetByIdAsync(permissionOperation.PermissionId);
                if (permission == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "Permissão não encontrada" })
                        .WithCode(404)
                        .Build();

                // Validar se a operação existe
                var operation = await _operationRepository.GetByIdAsync(permissionOperation.OperationId);
                if (operation == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "Operação não encontrada" })
                        .WithCode(404)
                        .Build();

                // Verificar se a relação já existe
                var existingRelation = await _permissionOperationRepository.GetByPermissionAndOperationAsync(
                    permissionOperation.PermissionId, permissionOperation.OperationId);
                
                if (existingRelation != null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "Relação já existe entre a permissão e operação" })
                        .WithCode(409)
                        .Build();

                var entity = _mapper.Map<PermissionOperationEntity>(permissionOperation);
                entity.CreatedAt = DateTime.Now;

                var createdEntity = await _permissionOperationRepository.AddAsync(entity);
                var dto = _mapper.Map<PermissionOperationDTO>(createdEntity);
                
                return ResponseBuilder<PermissionOperationDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionOperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Cria múltiplas relações Permissão-Operação
        /// </summary>
        /// <param name="permissionOperations"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionOperationDTO>>> CreatePermissionOperationsBulkAsync(PermissionOperationBulkDTO permissionOperations)
        {
            try
            {
                var results = new List<PermissionOperationDTO>();

                // Validar se a permissão existe
                var permission = await _permissionRepository.GetByIdAsync(permissionOperations.PermissionId);
                if (permission == null)
                    return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                        .Fail(new ErrorDTO { Message = "Permissão não encontrada" })
                        .WithCode(404)
                        .Build();

                foreach (var operationId in permissionOperations.OperationIds)
                {
                    // Validar se a operação existe
                    var operation = await _operationRepository.GetByIdAsync(operationId);
                    if (operation == null)
                        continue; // Pular operações que não existem

                    // Verificar se a relação já existe
                    var existingRelation = await _permissionOperationRepository.GetByPermissionAndOperationAsync(
                        permissionOperations.PermissionId, operationId);
                    
                    if (existingRelation != null)
                        continue; // Pular relações que já existem

                    var entity = new PermissionOperationEntity
                    {
                        Id = Guid.NewGuid(),
                        PermissionId = permissionOperations.PermissionId,
                        OperationId = operationId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    var createdEntity = await _permissionOperationRepository.AddAsync(entity);
                    var dto = _mapper.Map<PermissionOperationDTO>(createdEntity);
                    results.Add(dto);
                }

                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>.Ok(results).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionOperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Atualiza uma relação Permissão-Operação
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permissionOperation"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionOperationDTO>> UpdatePermissionOperationAsync(Guid id, PermissionOperationUpdateDTO permissionOperation)
        {
            try
            {
                var existingEntity = await _permissionOperationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<PermissionOperationDTO>
                        .Fail(new ErrorDTO { Message = "Relação Permissão-Operação não encontrada" })
                        .WithCode(404)
                        .Build();

                _mapper.Map(permissionOperation, existingEntity);
                existingEntity.UpdatedAt = DateTime.Now;

                await _permissionOperationRepository.UpdateAsync(existingEntity);
                var dto = _mapper.Map<PermissionOperationDTO>(existingEntity);
                
                return ResponseBuilder<PermissionOperationDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionOperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Remove uma relação Permissão-Operação (soft delete)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeletePermissionOperationAsync(Guid id)
        {
            try
            {
                var existingEntity = await _permissionOperationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Relação Permissão-Operação não encontrada" })
                        .WithCode(404)
                        .Build();

                existingEntity.IsActive = false;
                existingEntity.UpdatedAt = DateTime.Now;

                await _permissionOperationRepository.UpdateAsync(existingEntity);
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Remove todas as relações de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteAllByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                var result = await _permissionOperationRepository.RemoveAllByPermissionIdAsync(permissionId);
                return ResponseBuilder<bool>.Ok(result).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Remove relações específicas de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds)
        {
            try
            {
                var result = await _permissionOperationRepository.RemoveByPermissionAndOperationsAsync(permissionId, operationIds);
                return ResponseBuilder<bool>.Ok(result).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
    }
}