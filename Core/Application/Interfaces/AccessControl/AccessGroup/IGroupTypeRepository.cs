﻿using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup
{
    /// <summary>
    /// Interface para o repositório de tipos de grupo
    /// </summary>
    public interface IGroupTypeRepository : IBaseRepository<GroupTypeEntity> { }
}
