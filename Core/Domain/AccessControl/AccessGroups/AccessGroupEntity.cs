using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups;
using Authenticator.API.Core.Domain.AccessControl.RoleAccessGroups;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.AccessGroups;

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
    /// Se o grupo está ativo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Data de exclusão (soft delete)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<AccountAccessGroupEntity> AccountAccessGroups { get; set; } = new List<AccountAccessGroupEntity>();
    public virtual ICollection<RoleAccessGroupEntity> RoleAccessGroups { get; set; } = new List<RoleAccessGroupEntity>();
}