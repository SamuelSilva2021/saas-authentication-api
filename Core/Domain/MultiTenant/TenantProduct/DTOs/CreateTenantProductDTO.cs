using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class CreateTenantProductDTO
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Version { get; set; } = "1.0.0";

        [MaxLength(20)]
        public string Status { get; set; } = "active";

        public string? ConfigurationSchema { get; set; }

        [MaxLength(50)]
        public string PricingModel { get; set; } = "subscription";

        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; } = 0.00m;

        [Range(0, double.MaxValue)]
        public decimal SetupFee { get; set; } = 0.00m;
    }
}
