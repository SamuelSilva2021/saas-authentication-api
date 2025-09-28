using Authenticator.API.Core.Domain.AccessControl.Applications;
using Authenticator.API.Core.Domain.AccessControl.Permissions;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Modules;

/// <summary>
/// Módulo/funcionalidade do sistema
/// </summary>
public class ModuleEntity
{
    /// <summary>
    /// ID único do módulo
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do módulo
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do módulo
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// URL do módulo
    /// </summary>
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Chave única do módulo para identificação
    /// </summary>
    [MaxLength(100)]
    public string? ModuleKey { get; set; }

    /// <summary>
    /// Código numérico do módulo
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// ID da aplicação à qual o módulo pertence
    /// </summary>
    public Guid? ApplicationId { get; set; }

    /// <summary>
    /// ID do tipo de módulo
    /// </summary>
    public Guid? ModuleTypeId { get; set; }

    /// <summary>
    /// Tipo do módulo
    /// </summary>
    public ModuleTypeEntity ModuleType { get; set; } = null!;

    /// <summary>
    /// Se o módulo está ativo
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
    public virtual ApplicationEntity? Application { get; set; }
    public virtual ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
}