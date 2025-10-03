using Authenticator.API.Infrastructure.Mapper.AccessControl.AccessGroup;
using Authenticator.API.Infrastructure.Mapper.AccessControl.Permissions;
using Authenticator.API.Infrastructure.Mapper.AccessControl.UserAccount;
using Authenticator.API.Infrastructure.Mapper.MultiTenant;
using Authenticator.API.Infrastructure.Mapper.Tenant;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace Authenticator.API.Infrastructure.Configurations
{
    /// <summary>  
    /// AutoMapper configuration class  
    /// </summary>  
    [ExcludeFromCodeCoverage]
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new UserAccountProfile());
                cfg.AddProfile(new TenantProfile());
                cfg.AddProfile(new TenantProductProfile());
                cfg.AddProfile(new PlanProfile());
                cfg.AddProfile(new SubscriptionProfile());
                cfg.AddProfile<AccessGroupProfile>();
                cfg.AddProfile<PermissionProfile>();
                cfg.AddProfile<PermissionOperationProfile>();
            });
        }
    }
}
