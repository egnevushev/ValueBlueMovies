using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WebApi.Authentication;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthConfiguration>(configuration.GetSection(AuthConfiguration.SectionName));
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            const string id = "ApiKey";
            c.AddSecurityDefinition(id, new OpenApiSecurityScheme
            {
                Description = "Api Key for access the admin functionality",
                Type = SecuritySchemeType.ApiKey,
                Name = AuthConfiguration.ApiKeyHeaderName,
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = id
                },
                In = ParameterLocation.Header
            };
            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, new List<string>() }
            };
            c.AddSecurityRequirement(requirement);
        });

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        services.AddFluentValidationAutoValidation();

        return services;
    }

    public static IServiceCollection AddProblemDetailsHandler(this IServiceCollection services) =>
        services.AddProblemDetails(setup => { setup.IncludeExceptionDetails = (_, _) => false; });
}