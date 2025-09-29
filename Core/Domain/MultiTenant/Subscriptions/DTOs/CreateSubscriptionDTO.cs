using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs
{
    public class CreateSubscriptionDTO
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public Guid PlanId { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "active";

        public DateTime? TrialEndsAt { get; set; }

        [Required]
        public DateTime CurrentPeriodStart { get; set; }

        [Required]
        public DateTime CurrentPeriodEnd { get; set; }

        public bool CancelAtPeriodEnd { get; set; } = false;

        [Range(0, double.MaxValue)]
        public decimal? CustomPricing { get; set; }

        public string? UsageLimits { get; set; }
    }
}
