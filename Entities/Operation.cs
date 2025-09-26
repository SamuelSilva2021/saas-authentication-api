namespace Authenticator.API.Entities;

/// <summary>
/// Operações que podem ser realizadas no sistema
/// </summary>
public class Operation
{
    /// <summary>
    /// ID da operação
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome da operação
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da operação
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Valor binário da operação para controle de bits
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Se a operação está ativa
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
    /// <summary>
    /// Relações com permissões
    /// </summary>
    public virtual ICollection<PermissionOperation> PermissionOperations { get; set; } = new List<PermissionOperation>();
}