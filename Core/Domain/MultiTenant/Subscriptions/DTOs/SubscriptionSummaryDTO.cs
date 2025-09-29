﻿namespace Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs
{
    public class SubscriptionSummaryDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid PlanId { get; set; }
        public string Status { get; set; } = "active";
        public DateTime? TrialEndsAt { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public bool CancelAtPeriodEnd { get; set; } = false;
        public DateTime? CancelledAt { get; set; }
        public decimal? CustomPricing { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
