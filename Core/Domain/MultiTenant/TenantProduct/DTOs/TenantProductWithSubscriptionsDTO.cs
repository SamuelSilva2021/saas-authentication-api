using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;

namespace Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs
{
    public class TenantProductWithSubscriptionsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string Status { get; set; } = "active";
        public string PricingModel { get; set; } = "subscription";
        public decimal BasePrice { get; set; } = 0.00m;
        public decimal SetupFee { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; }

        public ICollection<SubscriptionSummaryDTO> Subscriptions { get; set; } = [];
        public int TotalSubscriptionsCount => Subscriptions.Count;
        public int ActiveSubscriptionsCount => Subscriptions.Count(s => s.Status == "active");
    }
}
