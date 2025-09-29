using Authenticator.API.Infrastructure.Mapper.AccessGroup;
using Authenticator.API.Infrastructure.Mapper.Module;
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
                cfg.AddProfile(new GroupTypeProfile());
                cfg.AddProfile(new AccessGroupProfile());
                cfg.AddProfile(new ModuleTypeProfile());
            });
        }
    }
}
