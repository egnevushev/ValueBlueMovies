using Domain.Exceptions;
using Domain.MovieSearchProviders;
using Domain.Repositories;
using Domain.Sources;
using Infrastructure.Cache;
using Infrastructure.MovieSources.Omdb;
using Infrastructure.MovieSources.Omdb.Configuration;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Configuration;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Driver;
using Polly;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAllMovieSources()
            .ConfigureOmdbMovieSource(configuration)
            .AddMongoRepository(configuration)
            .AddMovieCache(configuration)
            .AddMapster();

        return services;
    }

    private static IServiceCollection AddMongoRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuditDbConfiguration>(configuration.GetSection(AuditDbConfiguration.SectionName));
        
        var options = configuration.GetSectionAs<AuditDbConfiguration>(AuditDbConfiguration.SectionName);
        services.AddSingleton<IMongoClient>(new MongoClient(options.ConnectionString));
        services.AddMigration(new MongoMigrationSettings
        {
            ConnectionString = options.ConnectionString,
            Database = options.DatabaseName
        });
        
        services.AddScoped<IAuditRepository, AuditRepository>();

        return services;
    }
    
    private static IServiceCollection AddAllMovieSources(this IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(classes => classes.AssignableTo<IMovieSource>())
            .As<IMovieSource>()
            .WithScopedLifetime()
        );

    private static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
    
    private static IServiceCollection AddMovieCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheConfiguration>(configuration.GetSection(CacheConfiguration.SectionName));
        services.AddSingleton<IMovieCacheStrategy, InMemoryCacheStrategy>();
        services.AddMemoryCache(setup => 
        {
            var options = configuration.GetSectionAs<CacheConfiguration>(CacheConfiguration.SectionName);
            setup.SizeLimit = options.CacheSize;
        });
        
        return services;
    }

    private static IServiceCollection ConfigureOmdbMovieSource(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OmdbConfiguration>(configuration.GetSection(OmdbConfiguration.SectionName));

        var options = configuration.GetSectionAs<OmdbConfiguration>(OmdbConfiguration.SectionName);
        
        // register HttpClient for OmdbMovieSource with RetryPolicy
        services.AddHttpClient(OmdbMovieSource.HttpClientName, client =>
            {
                DomainException.ThrowIfNull(options.BaseAddress);
                client.Timeout = options.TimeOut;
                client.BaseAddress = options.BaseAddress;
            })
            .AddTransientHttpErrorPolicy(policy => 
                policy.WaitAndRetryAsync(options.RetryCount, _ => options.WaitBetweenRetry));

        return services;
    }

    private static T GetSectionAs<T>(this IConfiguration configuration, string name) => 
        configuration.GetSection(name).Get<T>() ?? throw new Exception($"{typeof(T).Name} doesn't exist");
}