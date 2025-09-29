﻿using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.MultiTenant
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            // SubscriptionEntity -> SubscriptionDto
            CreateMap<SubscriptionEnity, SubscriptionDTO>();

            // SubscriptionEntity -> SubscriptionSummaryDto
            CreateMap<SubscriptionEnity, SubscriptionSummaryDTO>();

            // CreateSubscriptionDto -> SubscriptionEntity
            CreateMap<CreateSubscriptionDTO, SubscriptionEnity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CancelledAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Plan, opt => opt.Ignore());

            // UpdateSubscriptionDto -> SubscriptionEnity
            CreateMap<UpdateSubscriptionDTO, SubscriptionEnity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.PlanId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Plan, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));

        }
    }
}
