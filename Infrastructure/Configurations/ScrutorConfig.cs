﻿using Scrutor;

namespace Authenticator.API.Infrastructure.Configurations
{
    public static class ScrutorConfig
    {
        public static void AddConfigureScrutor(this IServiceCollection services)
        {
            string[] namespaces = ["Authenticator.API"];

            services.Scan(
                scan => scan
                    .FromApplicationDependencies()
                        .AddClasses(classes => classes.InNamespaces(namespaces))
                        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
            );
        }
    }
}
