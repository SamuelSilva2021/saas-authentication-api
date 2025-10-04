using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.AccessControl.Roles.Entities;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.RolePermissions
{
    /// <summary>
    /// Interface do repositório de relações Role-Permission
    /// </summary>
    public interface IRolePermissionRepository : IBaseRepository<RolePermissionEntity>
    {
        /// <summary>
        /// Busca relações por ID do role
        /// </summary>
        Task<IEnumerable<RolePermissionEntity>> GetByRoleIdAsync(Guid roleId);

        /// <summary>
        /// Busca relações por ID da permissão
        /// </summary>
        Task<IEnumerable<RolePermissionEntity>> GetByPermissionIdAsync(Guid permissionId);

        /// <summary>
        /// Busca uma relação específica entre role e permissão
        /// </summary>
        Task<RolePermissionEntity?> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId);

        /// <summary>
        /// Remove todas as relações de um role (soft delete)
        /// </summary>
        Task<bool> RemoveAllByRoleIdAsync(Guid roleId);

        /// <summary>
        /// Remove relações específicas (soft delete)
        /// </summary>
        Task<bool> RemoveByRoleAndPermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds);
    }
}