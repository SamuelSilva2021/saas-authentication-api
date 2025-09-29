using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs
{
    public class UpdatePlanDTO
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [MaxLength(20)]
        public string? BillingCycle { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxUsers { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxStorageGb { get; set; }

        public string? Features { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public int? SortOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
