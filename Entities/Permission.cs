using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Entities;

/// <summary>
/// Permissão no sistema
/// </summary>
public class Permission
{
    /// <summary>
    /// ID único da permissão
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da permissão (derivado do módulo)
    /// </summary>
    public string Name => Module?.Name ?? $"Permission_{Id}";

    /// <summary>
    /// Descrição da permissão (derivada do módulo)
    /// </summary>
    public string Description => Module?.Description ?? string.Empty;

    /// <summary>
    /// Código da permissão (derivado do módulo)
    /// </summary>
    public string? Code => Module?.ModuleKey;

    /// <summary>
    /// ID do papel que possui esta permissão
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// ID do módulo ao qual a permissão se aplica
    /// </summary>
    public Guid? ModuleId { get; set; }

    /// <summary>
    /// Se a permissão está ativa
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
    public virtual Role? Role { get; set; }
    public virtual Module? Module { get; set; }
    public virtual ICollection<PermissionOperation> PermissionOperations { get; set; } = new List<PermissionOperation>();
}