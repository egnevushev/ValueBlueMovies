using Domain;
using Domain.Sources;
using Infrastructure.Cache;
using Infrastructure.MovieSources.Omdb;
using Infrastructure.MovieSources.Omdb.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services
            .RegisterAllSources()
            .ConfigureOmdbMovieSource(configuration)
            .ConfigureMovieCache(configuration);

        return services;
    }

    private static IServiceCollection RegisterAllSources(this IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(classes => classes.AssignableTo<IMovieSource>())
            .As<IMovieSource>()
            .WithScopedLifetime()
        );
    
    private static IServiceCollection ConfigureMovieCache(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.Configure<CacheConfiguration>(configuration.GetSection(CacheConfiguration.SectionName));
        services.AddSingleton<IMovieCache, InMemoryCache>();
        services.AddMemoryCache(setup => 
        {
            var options = configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>()
                          ?? throw new Exception($"{nameof(CacheConfiguration)} doesn't exist");

            setup.SizeLimit = options.CacheSize;
        });
        
        return services;
    }

    private static IServiceCollection ConfigureOmdbMovieSource(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.Configure<OmdbConfiguration>(configuration.GetSection(OmdbConfiguration.SectionName));

        var options = configuration.GetSection(OmdbConfiguration.SectionName).Get<OmdbConfiguration>()
                      ?? throw new Exception($"{nameof(OmdbConfiguration)} doesn't exist");
        
        // register HttpClient for OmdbMovieSource with RetryPolicy
        services.AddHttpClient(OmdbMovieSource.HttpClientName, client =>
            {
                client.Timeout = options.TimeOut;
                client.BaseAddress = options.BaseAddress
                                     ?? throw new Exception($"{nameof(OmdbConfiguration)}:{nameof(options.BaseAddress)} should be set");
            })
            .AddTransientHttpErrorPolicy(policy => 
                policy.WaitAndRetryAsync(options.RetryCount, _ => options.WaitBetweenRetry));

        return services;
    }
}