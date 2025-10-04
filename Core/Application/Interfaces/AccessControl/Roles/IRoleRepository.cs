﻿using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.AccessControl.Roles;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Roles
{
    /// <summary>
    /// Interface para o repositório de Roles
    /// </summary>
    public interface IRoleRepository : IBaseRepository<RoleEntity>
    {
        /// <summary>
        /// Obtém roles por tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleEntity>> GetAllByTenantAsync(Guid tenantId);
    }
}