using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Entities;

/// <summary>
/// Representa um plano de assinatura disponível
/// </summary>
public class Plan
{
    /// <summary>
    /// ID único do plano
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do plano
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Slug do plano
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do plano
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Preço do plano
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Ciclo de cobrança
    /// </summary>
    [MaxLength(20)]
    public string BillingCycle { get; set; } = "monthly";

    /// <summary>
    /// Máximo de usuários
    /// </summary>
    public int MaxUsers { get; set; } = 1;

    /// <summary>
    /// Máximo de armazenamento em GB
    /// </summary>
    public int MaxStorageGb { get; set; } = 1;

    /// <summary>
    /// Recursos inclusos no plano em JSON
    /// </summary>
    public string? Features { get; set; }

    /// <summary>
    /// Status do plano
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "active";

    /// <summary>
    /// Ordem de exibição
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Se o plano está ativo
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
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}