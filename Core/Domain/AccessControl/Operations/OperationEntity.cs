using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;

namespace Authenticator.API.Core.Domain.AccessControl.Operations;

/// <summary>
/// Operações que podem ser realizadas no sistema
/// </summary>
public class OperationEntity
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
    /// Valor da operação (ex: 'CREATE', 'READ', 'UPDATE','DELETE')
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Se a operação está ativa
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
    /// <summary>
    /// Relações com permissões
    /// </summary>
    public virtual ICollection<PermissionOperationEntity> PermissionOperations { get; set; } = new List<PermissionOperationEntity>();
}