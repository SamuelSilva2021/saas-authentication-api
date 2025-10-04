using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations
{
    /// <summary>
    /// Interface do repositório de relações Permissão-Operação
    /// </summary>
    public interface IPermissionOperationRepository : IBaseRepository<PermissionOperationEntity>
    {
        /// <summary>
        /// Busca relações por ID da permissão
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionOperationEntity>> GetByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Busca relações por ID da operação
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionOperationEntity>> GetByOperationIdAsync(Guid operationId);

        /// <summary>
        /// Busca uma relação específica entre permissão e operação
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<PermissionOperationEntity?> GetByPermissionAndOperationAsync(Guid permissionId, Guid operationId);

        /// <summary>
        /// Remove todas as relações de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<bool> RemoveAllByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Remove relações específicas (soft delete)
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="operationIds"></param>
        /// <returns></returns>
        Task<bool> RemoveByPermissionAndOperationsAsync(Guid permissionId, IEnumerable<Guid> operationIds);
    }
}