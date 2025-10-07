using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Modules.Entities;

namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs
{
    public class RolesBasicDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public List<ModuleBasicDTO> Modules { get; set; } = new();
    }
}
