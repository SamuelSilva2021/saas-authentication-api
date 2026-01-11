using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;
using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct;

/// <summary>
/// Produto/aplicação oferecida pela plataforma
/// </summary>
public class TenantProductEntity
{
    /// <summary>
    /// ID único do produto
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Slug do produto
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do produto
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Categoria do produto
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Versão do produto
    /// </summary>
    [MaxLength(20)]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Status do produto
    /// </summary>
    [MaxLength(20)]
    public EProductStatus Status { get; set; } = EProductStatus.Ativo;

    /// <summary>
    /// Schema de configuração em JSON
    /// </summary>
    public string? ConfigurationSchema { get; set; }

    /// <summary>
    /// Modelo de preços
    /// </summary>
    [MaxLength(50)]
    public string PricingModel { get; set; } = "subscription";

    /// <summary>
    /// Preço base
    /// </summary>
    public decimal BasePrice { get; set; } = 0.00m;

    /// <summary>
    /// Taxa de configuração
    /// </summary>
    public decimal SetupFee { get; set; } = 0.00m;

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<SubscriptionEntity> Subscriptions { get; set; } = new List<SubscriptionEntity>();
}