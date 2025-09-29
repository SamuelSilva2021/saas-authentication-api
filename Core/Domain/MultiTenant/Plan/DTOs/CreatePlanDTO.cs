using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class CreatePlanDTO
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(20)]
        public string BillingCycle { get; set; } = "monthly";

        [Range(1, int.MaxValue)]
        public int MaxUsers { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int MaxStorageGb { get; set; } = 1;

        public string? Features { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "active";

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
