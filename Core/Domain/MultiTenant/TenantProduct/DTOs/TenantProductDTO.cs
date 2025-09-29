namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class TenantProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string Status { get; set; } = "active";
        public string? ConfigurationSchema { get; set; }
        public string PricingModel { get; set; } = "subscription";
        public decimal BasePrice { get; set; } = 0.00m;
        public decimal SetupFee { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
    }
}
