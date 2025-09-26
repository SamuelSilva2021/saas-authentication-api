using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Entities;

/// <summary>
/// Módulo/funcionalidade do sistema
/// </summary>
public class Module
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
    public int? Code { get; set; }

    /// <summary>
    /// ID da aplicação à qual o módulo pertence
    /// </summary>
    public Guid? ApplicationId { get; set; }

    /// <summary>
    /// ID do tipo de módulo
    /// </summary>
    public Guid? ModuleTypeId { get; set; }

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
    public virtual Application? Application { get; set; }
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}