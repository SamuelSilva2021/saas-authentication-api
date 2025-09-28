using Authenticator.API.Core.Application.Implementation;
using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Configurations;
using Authenticator.API.Infrastructure.Data;
using Authenticator.API.Infrastructure.Providers;
using Authenticator.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Text;

namespace Authenticator.API.Infrastructure.Extensions;

/// <summary>
/// Extensões para configuração de serviços
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura os bancos de dados
    /// </summary>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var accessControlDataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("AccessControlDatabase"));
        accessControlDataSourceBuilder.EnableDynamicJson();
        var accessControlDataSource = accessControlDataSourceBuilder.Build();

        services.AddDbContext<AccessControlDbContext>(options =>
            options.UseNpgsql(accessControlDataSource)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

        var multiTenantDataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("MultiTenantDatabase"));
        multiTenantDataSourceBuilder.EnableDynamicJson();
        var multiTenantDataSource = multiTenantDataSourceBuilder.Build();

        services.AddDbContext<MultiTenantDbContext>(options =>
            options.UseNpgsql(multiTenantDataSource)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        return services;
    }

    /// <summary>
    /// Configura a autenticação JWT
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettings);

        var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Falha na autenticação JWT: {Error}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    var userId = context.Principal?.Identity?.Name;
                    logger.LogInformation("Token JWT validado para usuário: {UserId}", userId);
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configura os serviços de aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddConfigureScrutor();
        services.AddAutoMapperConfig();
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Configura o Swagger/OpenAPI
    /// </summary>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Authentication API",
                Version = "1.0.0",
                Description = "API de autenticação e autorização com JWT e RBAC para o ecossistema Pedeja",
                Contact = new OpenApiContact
                {
                    Name = "Equipe Pedeja",
                    Email = "contato@pedeja.com",
                    Url = new Uri("https://pedeja.com")
                },
                License = new OpenApiLicense
                {
                    Name = "Proprietário",
                    Url = new Uri("https://pedeja.com/licenca")
                },
                TermsOfService = new Uri("https://pedeja.com/termos")
            });

            // Configuração para autenticação JWT no Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"Autenticação JWT usando o esquema Bearer.
                
                    **Como usar:**
                    1. Faça login no endpoint `/api/auth/login`
                    2. Copie o `accessToken` da resposta
                    3. Cole o token no campo abaixo (apenas o token, sem 'Bearer ')
                    4. Clique em 'Authorize' e teste os endpoints protegidos
                    
                    **Exemplo de token:**
                    eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Inclui comentários XML na documentação
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            c.EnableAnnotations();
            c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

            c.SchemaFilter<ExampleSchemaFilter>();
        });

        return services;
    }

    /// <summary>
    /// Configura CORS
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", builder =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };

                if (allowedOrigins.Contains("*"))
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                }
                else
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                }
            });
        });

        return services;
    }
}