using System.Data;

namespace Authenticator.API.Core.Domain.AccessControl.Roles
{
    public class RoleTypeEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
    }
}
