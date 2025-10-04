using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Roles;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.Roles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            // CreateDTO -> Entity
            CreateMap<RoleCreateDTO, RoleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RoleAccessGroups, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            // UpdateDTO -> Entity
            CreateMap<RoleUpdateDTO, RoleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.RoleAccessGroups, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            // Entity -> DTO
            CreateMap<RoleEntity, RoleDTO>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.RolePermissions != null && src.RolePermissions.Any(rp => rp.IsActive)
                        ? src.RolePermissions
                            .Where(rp => rp.IsActive && rp.Permission != null)
                            .Select(rp => rp.Permission)
                        : new List<PermissionEntity>()))
                .ForMember(dest => dest.AccessGroups, opt => opt.MapFrom(src =>
                    src.RoleAccessGroups != null && src.RoleAccessGroups.Any(rag => rag.IsActive)
                        ? src.RoleAccessGroups
                            .Where(rag => rag.IsActive && rag.AccessGroup != null)
                            .Select(rag => rag.AccessGroup)
                        : new List<AccessGroupEntity>()));
        }
    }
}