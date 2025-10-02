using Authenticator.API.Core.Domain.AccessControl.Permissions;

namespace Authenticator.API.Core.Domain.AccessControl.Roles.Entities
{
    /// <summary>
    /// Entidade que representa a relação entre papéis e permissões
    /// </summary>
    public class RolePermissionEntity
    {
        /// <summary>
        /// ID único da relação papel-permissão
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// ID do papel
        /// </summary>
        public Guid RoleId { get; set; }
        /// <summary>
        /// ID da permissão
        /// </summary>
        public Guid PermissionId { get; set; }
        /// <summary>
        /// Se a relação está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Navegação para o papel
        /// </summary>
        public RoleEntity Role { get; set; } = null!;
        /// <summary>
        /// Navegação para a permissão
        /// </summary>
        public PermissionEntity Permission { get; set; } = null!;
    }
}
