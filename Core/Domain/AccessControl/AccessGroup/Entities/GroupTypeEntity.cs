﻿namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities
{
    /// <summary>
    /// Tipo de Grupo de Acesso
    /// </summary>
    public class GroupTypeEntity
    {
        /// <summary>
        /// Identificador único do tipo de grupo de acesso
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nome do tipo de grupo de acesso
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Descrição do tipo de grupo de acesso
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Código do tipo de grupo de acesso (ex: "SYSTEM", "CUSTOM")
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// Indica se o tipo de grupo de acesso está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Data de criação do tipo de grupo de acesso
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Data da última atualização do tipo de grupo de acesso
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Grupos de Acesso associados a este tipo
        /// </summary>
        public ICollection<AccessGroupEntity> AccessGroups { get; set; } = new List<AccessGroupEntity>();
    }
}
