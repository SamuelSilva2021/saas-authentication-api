﻿using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.AccessGroup
{
    public class AccessGroupProfile : Profile
    {
        public AccessGroupProfile()
        {
            // Entity -> DTO
            CreateMap<AccessGroupEntity, AccessGroupDTO>()
                .ForMember(dest => dest.GroupTypeName,
                           opt => opt.MapFrom(src => src.GroupType != null ? src.GroupType.Name : null));

            // DTO -> Entity
            CreateMap<CreateAccessGroupDTO, AccessGroupEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateAccessGroupDTO, AccessGroupEntity>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
