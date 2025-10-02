using Authenticator.API.Core.Domain.AccessControl.Roles;

namespace Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities
{
    public class AccessGroupRoleEntity
    {
        public Guid GroupId { get; set; }
        public Guid RoleId { get; set; }

        public AccessGroupEntity AccessGroup { get; set; } = null!;
        public RoleEntity Role { get; set; } = null!;
    }
}
