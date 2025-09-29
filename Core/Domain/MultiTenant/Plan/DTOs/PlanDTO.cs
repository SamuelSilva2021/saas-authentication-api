namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class PlanDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = "monthly";
        public int MaxUsers { get; set; } = 1;
        public int MaxStorageGb { get; set; } = 1;
        public string? Features { get; set; }
        public string Status { get; set; } = "active";
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal MonthlyRecurringRevenue { get; set; }
    }
}
