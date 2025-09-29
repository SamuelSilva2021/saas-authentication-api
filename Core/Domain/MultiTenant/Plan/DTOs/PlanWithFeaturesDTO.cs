namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanWithFeaturesDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = "monthly";
        public int MaxUsers { get; set; } = 1;
        public int MaxStorageGb { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        public Dictionary<string, object> Features { get; set; } = new();
        public List<string> FeatureList { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
