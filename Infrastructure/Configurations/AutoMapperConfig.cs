using Authenticator.API.Infrastructure.Mapper;
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
            });
        }
    }
}
