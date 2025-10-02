using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.Etities;
using Authenticator.API.Core.Domain.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;

/// <summary>
/// Grupo de acesso para usuários
/// </summary>
public class AccessGroupEntity
{
    /// <summary>
    /// ID único do grupo
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do grupo
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do grupo de acesso
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Código único do grupo
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// ID do tenant
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// ID do tipo de grupo
    /// </summary>
    public Guid? GroupTypeId { get; set; }

    /// <summary>
    /// Tipo do grupo
    /// </summary>
    public GroupTypeEntity GroupType { get; set; } = null!;

    /// <summary>
    /// Se o grupo está ativo
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

    // Navigation properties
    public TenantEntity Tenant { get; set; } = null!;
    public virtual ICollection<AccountAccessGroupEntity> AccountAccessGroups { get; set; } = new List<AccountAccessGroupEntity>();
    public virtual ICollection<RoleAccessGroupEntity> RoleAccessGroups { get; set; } = new List<RoleAccessGroupEntity>();

}