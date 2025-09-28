using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper
{
    /// <summary>
    /// Perfil de mapeamento para GroupType
    /// </summary>
    public class GroupTypeProfile : Profile
    {
        /// <summary>
        /// Construtor que configura os mapeamentos
        /// </summary>
        public GroupTypeProfile()
        {
            // Entidade → DTO (resposta)
            CreateMap<GroupTypeEntity, GroupTypeDTO>();

            // CreateDTO → Entidade (para criação)
            CreateMap<GroupTypeCreateDTO, GroupTypeEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AccessGroups, opt => opt.Ignore());

            // UpdateDTO → Entidade (para atualização)
            CreateMap<GroupTypeUpdateDTO, GroupTypeEntity>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AccessGroups, opt => opt.Ignore());

            // Entidade → ApiResponse<DTO>
            CreateMap<GroupTypeEntity, ApiResponse<GroupTypeDTO>>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(_ => "Tipo de grupo criado com sucesso."))
                .ForMember(dest => dest.Errors, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
