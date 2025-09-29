namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class TenantProductSummaryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string Status { get; set; } = "active";
        public string PricingModel { get; set; } = "subscription";
        public decimal BasePrice { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; }
    }
}
