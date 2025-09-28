using Authenticator.API.Core.Domain.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.RoleAccessGroups;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Roles;

/// <summary>
/// Papel/função no sistema
/// </summary>
public class RoleEntity
{
    /// <summary>
    /// ID único do papel
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do role ex: Administrador, Usuário
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do role ex: Papel com permissões administrativas, Papel com permissões de usuário comum
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Código único do role
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// ID do tenant
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// ID do tipo de papel
    /// </summary>
    public Guid? RoleTypeId { get; set; }

    /// <summary>
    /// Tipo do papel
    /// </summary>
    public RoleTypeEntity RoleType { get; set; } = default!;
    /// <summary>
    /// ID da aplicação
    /// </summary>
    public Guid? ApplicationId { get; set; }

    /// <summary>
    /// Se o role está ativo
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
    /// Data de exclusão (soft delete)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
    public virtual ICollection<RoleAccessGroupEntity> RoleAccessGroups { get; set; } = new List<RoleAccessGroupEntity>();
}