using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.Module
{
    public class ModuleTypeProfile: Profile
    {
        public ModuleTypeProfile()
        {
            // Entity → Response
            CreateMap<ModuleTypeEntity, ModuleTypeDTO>();

            // Create DTO → Entity
            CreateMap<ModuleTypeCreateDTO, ModuleTypeEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Modules, opt => opt.Ignore());

            // Update DTO → Entity
            CreateMap<ModuleTypeUpdateDTO, ModuleTypeEntity>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Modules, opt => opt.Ignore());
        }
    }
}
