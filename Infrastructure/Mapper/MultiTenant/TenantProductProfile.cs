﻿using Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs;
using Authenticator.API.Core.Domain.MultiTenant.TenantProduct;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.MultiTenant
{
    public class TenantProductProfile : Profile
    {
        public TenantProductProfile()
        {

            // TenantProductEntity -> TenantProductSummaryDto
            CreateMap<TenantProductEntity, TenantProductSummaryDTO>();

            // TenantProductEntity -> TenantProductWithSubscriptionsDto
            CreateMap<TenantProductEntity, TenantProductWithSubscriptionsDTO>();

            // CreateTenantProductDto -> TenantProductEntity
            CreateMap<CreateTenantProductDTO, TenantProductEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore());

            // UpdateTenantProductDto -> TenantProductEntity
            CreateMap<UpdateTenantProductDTO, TenantProductEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null && !(srcMember is string str && string.IsNullOrWhiteSpace(str))));

            // TenantProductEntity -> TenantProductDto
            CreateMap<TenantProductEntity, TenantProductDTO>()
                .ForMember(dest => dest.TotalSubscriptions, opt => opt.MapFrom(src => src.Subscriptions.Count))
                .ForMember(dest => dest.ActiveSubscriptions, opt => opt.MapFrom(src =>
                    src.Subscriptions.Count(s => s.Status == "active")));

            // TenantProductEntity -> TenantProductConfigurationDto
            CreateMap<TenantProductEntity, TenantProductConfigurationDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RequiresConfiguration,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ConfigurationSchema)));
        }
    }
}
