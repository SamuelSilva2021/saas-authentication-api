using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class UpdateTenantProductDTO
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(20)]
        public string? Version { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public string? ConfigurationSchema { get; set; }

        [MaxLength(50)]
        public string? PricingModel { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? BasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SetupFee { get; set; }
    }
}
