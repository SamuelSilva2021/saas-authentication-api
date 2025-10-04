using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Domain.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Permissions
{
    /// <summary>
    /// Serviço para gerenciar permissões
    /// </summary>
    /// <param name="permissionRepository"></param>
    /// <param name="operationRepository"></param>
    /// <param name="mapper"></param>
    public class PermissionService(
        IPermissionRepository permissionRepository,
        IOperationRepository operationRepository,
        IMapper mapper) : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;
        private readonly IOperationRepository _operationRepository = operationRepository;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Obtém todas as permissões
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync()
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém todas as permissões paginadas
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PagedResponseDTO<PermissionDTO>>> GetAllPermissionsPagedAsync(int page, int limit)
        {
            try
            {
                var entities = await _permissionRepository.GetPagedAsync(
                    page: page,
                    pageSize: limit,
                    include: p => p
                        .Include(x => x.Module)
                        .Include(x => x.PermissionOperations)
                            .ThenInclude(po => po.Operation));

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities.Items);

                var pagedResponse = new PagedResponseDTO<PermissionDTO>
                {
                    Items = dtos,
                    TotalCount = entities.TotalCount,
                    Page = entities.Page,
                    PageSize = entities.PageSize,
                    TotalPages = entities.TotalPages
                };

                return ResponseBuilder<PagedResponseDTO<PermissionDTO>>.Ok(pagedResponse).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PagedResponseDTO<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém uma permissão pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionDTO>> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                var entity = await _permissionRepository.GetByIdAsync(id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                if (entity == null)
                    return ResponseBuilder<PermissionDTO>
                        .Fail(new ErrorDTO { Message = "Permissão não encontrada" })
                        .WithCode(404)
                        .Build();

                var dto = _mapper.Map<PermissionDTO>(entity);
                return ResponseBuilder<PermissionDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém permissões por módulo
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByModuleAsync(Guid moduleId)
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(
                    filter: p => p.ModuleId == moduleId,
                    include: p => p
                        .Include(x => x.Module)
                        .Include(x => x.PermissionOperations)
                            .ThenInclude(po => po.Operation));

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Obtém permissões por role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<PermissionDTO>>> GetPermissionsByRoleAsync(Guid roleId)
        {
            try
            {
                var entities = await _permissionRepository.GetAllAsync(
                    filter: p => p.RoleId == roleId,
                    include: p => p
                        .Include(x => x.Module)
                        .Include(x => x.PermissionOperations)
                            .ThenInclude(po => po.Operation));

                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(entities);
                return ResponseBuilder<IEnumerable<PermissionDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<PermissionDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Adiciona uma nova permissão
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionDTO>> AddPermissionAsync(PermissionCreateDTO permission)
        {
            try
            {
                var entity = _mapper.Map<PermissionEntity>(permission);
                entity.CreatedAt = DateTime.Now;

                var createdEntity = await _permissionRepository.AddAsync(entity);

                // Associar operações se fornecidas
                if (permission.OperationIds != null && permission.OperationIds.Any())
                {
                    await AssignOperationsToPermissionInternalAsync(createdEntity.Id, permission.OperationIds);
                }

                // Recarregar a entidade com includes
                var entityWithIncludes = await _permissionRepository.GetByIdAsync(createdEntity.Id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                var dto = _mapper.Map<PermissionDTO>(entityWithIncludes);
                return ResponseBuilder<PermissionDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Atualiza uma permissão
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<PermissionDTO>> UpdatePermissionAsync(Guid id, PermissionUpdateDTO permission)
        {
            try
            {
                var existingEntity = await _permissionRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<PermissionDTO>
                        .Fail(new ErrorDTO { Message = "Permissão não encontrada" })
                        .WithCode(404)
                        .Build();

                _mapper.Map(permission, existingEntity);
                existingEntity.UpdatedAt = DateTime.Now;

                await _permissionRepository.UpdateAsync(existingEntity);

                // Atualizar operações se fornecidas
                if (permission.OperationIds != null)
                {
                    // Remover todas as operações atuais
                    await RemoveAllOperationsFromPermissionInternalAsync(id);
                    
                    // Adicionar as novas operações
                    if (permission.OperationIds.Any())
                    {
                        await AssignOperationsToPermissionInternalAsync(id, permission.OperationIds);
                    }
                }

                // Recarregar a entidade com includes
                var entityWithIncludes = await _permissionRepository.GetByIdAsync(id, include: p => p
                    .Include(x => x.Module)
                    .Include(x => x.PermissionOperations)
                        .ThenInclude(po => po.Operation));

                var dto = _mapper.Map<PermissionDTO>(entityWithIncludes);
                return ResponseBuilder<PermissionDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PermissionDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        /// <summary>
        /// Deleta uma permissão
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeletePermissionAsync(Guid id)
        {
            try
            {
                var existingEntity = await _permissionRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Permissão não encontrada" })
                        .WithCode(404)
                        .Build();

                // Remover todas as operações associadas antes de deletar
                await RemoveAllOperationsFromPermissionInternalAsync(id);

                var result = await _permissionRepository.DeleteAsync(id);
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
        /// Associa operações a uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> AssignOperationsToPermissionAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                var result = await AssignOperationsToPermissionInternalAsync(permissionId, operationIds);
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
        /// Remove operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> RemoveOperationsFromPermissionAsync(Guid permissionId, List<Guid> operationIds)
        {
            try
            {
                var result = await RemoveOperationsFromPermissionInternalAsync(permissionId, operationIds);
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
        /// Método interno para associar operações a uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        private async Task<bool> AssignOperationsToPermissionInternalAsync(Guid permissionId, List<Guid> operationIds)
        {
            // Verificar se a permissão existe
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
                throw new ArgumentException("Permissão não encontrada");

            // Verificar se todas as operações existem
            foreach (var operationId in operationIds)
            {
                var operation = await _operationRepository.GetByIdAsync(operationId);
                if (operation == null)
                    throw new ArgumentException($"Operação com ID {operationId} não encontrada");
            }

            // Obter operações já associadas para evitar duplicatas
            var existingPermissionOperations = await _permissionRepository.GetAllAsync(
                filter: po => po.Id == permissionId,
                include: p => p.Include(x => x.PermissionOperations));

            var existingOperationIds = existingPermissionOperations
                .SelectMany(p => p.PermissionOperations)
                .Where(po => po.IsActive && po.DeletedAt == null)
                .Select(po => po.OperationId)
                .ToList();

            // Filtrar operações que já não estão associadas
            var newOperationIds = operationIds.Except(existingOperationIds).ToList();

            // Criar novos relacionamentos
            foreach (var operationId in newOperationIds)
            {
                var permissionOperation = new PermissionOperationEntity
                {
                    Id = Guid.NewGuid(),
                    PermissionId = permissionId,
                    OperationId = operationId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                // Assumindo que existe um repositório ou método para adicionar PermissionOperation
                // Como não foi implementado, vou usar uma abordagem através do contexto
                permission.PermissionOperations.Add(permissionOperation);
            }

            await _permissionRepository.UpdateAsync(permission);
            return true;
        }

        /// <summary>
        /// Método interno para remover operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        private async Task<bool> RemoveOperationsFromPermissionInternalAsync(Guid permissionId, List<Guid> operationIds)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId, include: p => p
                .Include(x => x.PermissionOperations));

            if (permission == null)
                throw new ArgumentException("Permissão não encontrada");

            var permissionOperationsToRemove = permission.PermissionOperations
                .Where(po => operationIds.Contains(po.OperationId) && po.IsActive && po.DeletedAt == null)
                .ToList();

            foreach (var permissionOperation in permissionOperationsToRemove)
            {
                permissionOperation.IsActive = false;
                permissionOperation.DeletedAt = DateTime.Now;
                permissionOperation.UpdatedAt = DateTime.Now;
            }

            await _permissionRepository.UpdateAsync(permission);
            return true;
        }

        /// <summary>
        /// Método interno para remover todas as operações de uma permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        private async Task<bool> RemoveAllOperationsFromPermissionInternalAsync(Guid permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId, include: p => p
                .Include(x => x.PermissionOperations));

            if (permission == null)
                return false;

            var activePermissionOperations = permission.PermissionOperations
                .Where(po => po.IsActive && po.DeletedAt == null)
                .ToList();

            foreach (var permissionOperation in activePermissionOperations)
            {
                permissionOperation.IsActive = false;
                permissionOperation.DeletedAt = DateTime.Now;
                permissionOperation.UpdatedAt = DateTime.Now;
            }

            await _permissionRepository.UpdateAsync(permission);
            return true;
        }
    }
}