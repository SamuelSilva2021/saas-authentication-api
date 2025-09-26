namespace Authenticator.API.Entities;

/// <summary>
/// Relacionamento entre permissão e operação
/// Define quais operações uma permissão permite executar
/// </summary>
public class PermissionOperation
{
    /// <summary>
    /// ID do relacionamento
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID da permissão
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// ID da operação
    /// </summary>
    public Guid OperationId { get; set; }

    /// <summary>
    /// Se a operação está ativa
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data de criação do relacionamento
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
    /// <summary>
    /// Permissão relacionada
    /// </summary>
    public virtual Permission Permission { get; set; } = null!;

    /// <summary>
    /// Operação relacionada
    /// </summary>
    public virtual Operation Operation { get; set; } = null!;
}