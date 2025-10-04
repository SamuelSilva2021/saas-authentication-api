using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.PermissionOperations
{
    /// <summary>
    /// Repositório para relações Permissão-Operação
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class PermissionOperationRepository(
        IDbContextProvider dbContextProvider,
        ILogger<PermissionOperationRepository> logger
        ) : BaseRepository<PermissionOperationEntity>(dbContextProvider), IPermissionOperationRepository
    {
        private readonly ILogger<PermissionOperationRepository> _logger = logger;

        /// <summary>
        /// Busca relações por ID da permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PermissionOperationEntity>> GetByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                return await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && po.IsActive && po.DeletedAt == null,
                    include: query => query
                        .Include(po => po.Permission)
                        .Include(po => po.Operation)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relações por permissão {PermissionId}", permissionId);
                throw;
            }
        }

        /// <summary>
        /// Busca relações por ID da operação
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PermissionOperationEntity>> GetByOperationIdAsync(Guid operationId)
        {
            try
            {
                return await GetAllAsync(
                    filter: po => po.OperationId == operationId && po.IsActive && po.DeletedAt == null,
                    include: query => query
                        .Include(po => po.Permission)
                        .Include(po => po.Operation)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relações por operação {OperationId}", operationId);
                throw;
            }
        }

        /// <summary>
        /// Busca uma relação específica entre permissão e operação
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<PermissionOperationEntity?> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && 
                                  po.OperationId == operationId && 
                                  po.IsActive && 
                                  po.DeletedAt == null,
                    include: query => query
                        .Include(po => po.Permission)
                        .Include(po => po.Operation)
                );

                return relations.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relação entre permissão {PermissionId} e operação {OperationId}", 
                    permissionId, operationId);
                throw;
            }
        }

        /// <summary>
        /// Remove todas as relações de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAllByPermissionIdAsync(Guid permissionId)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && po.IsActive && po.DeletedAt == null
                );

                foreach (var relation in relations)
                {
                    relation.IsActive = false;
                    relation.DeletedAt = DateTime.Now;
                    relation.UpdatedAt = DateTime.Now;
                    await UpdateAsync(relation);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover todas as relações da permissão {PermissionId}", permissionId);
                throw;
            }
        }

        /// <summary>
        /// Remove relações específicas (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        public async Task<bool> RemoveByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds)
        {
            try
            {
                var relations = await GetAllAsync(
                    filter: po => po.PermissionId == permissionId && 
                                  operationIds.Contains(po.OperationId) && 
                                  po.IsActive && 
                                  po.DeletedAt == null
                );

                foreach (var relation in relations)
                {
                    relation.IsActive = false;
                    relation.DeletedAt = DateTime.Now;
                    relation.UpdatedAt = DateTime.Now;
                    await UpdateAsync(relation);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover relações específicas da permissão {PermissionId}", permissionId);
                throw;
            }
        }
    }
}