﻿using Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs;
using Authenticator.API.Core.Domain.MultiTenant.Plan;
using AutoMapper;
using System.Text.Json;

namespace Authenticator.API.Infrastructure.Mapper.MultiTenant
{
    public class PlanProfile : Profile
    {
        public PlanProfile()
        {
            // CreatePlanDto -> PlanEntity  
            CreateMap<CreatePlanDTO, PlanEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore());

            // UpdatePlanDto -> PlanEntity  
            CreateMap<UpdatePlanDTO, PlanEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));

            // PlanEntity -> PlanDto  
            CreateMap<PlanEntity, PlanDTO>()
                .ForMember(dest => dest.TotalSubscriptions, opt => opt.MapFrom(src => src.Subscriptions.Count))
                .ForMember(dest => dest.ActiveSubscriptions, opt => opt.MapFrom(src =>
                    src.Subscriptions.Count(s => s.Status == "active")))
                .ForMember(dest => dest.MonthlyRecurringRevenue, opt => opt.MapFrom(src =>
                    src.Subscriptions.Where(s => s.Status == "active").Sum(s => s.CustomPricing ?? src.Price)));

            // PlanEntity -> PlanSummaryDto  
            CreateMap<PlanEntity, PlanSummaryDTO>();

            // PlanEntity -> PlanWithFeaturesDto  
            CreateMap<PlanEntity, PlanWithFeaturesDTO>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Features)
                        ? new Dictionary<string, object>()
                        : JsonSerializer.Deserialize<Dictionary<string, object>>(src.Features, new JsonSerializerOptions()) ?? new Dictionary<string, object>()))
                .ForMember(dest => dest.FeatureList, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Features)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(src.Features, new JsonSerializerOptions()).ToList() ?? new List<string>()));

        }
    }
}
