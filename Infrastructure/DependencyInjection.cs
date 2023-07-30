using Domain.Sources;
using Infrastructure.MovieSources.Omdb;
using Infrastructure.MovieSources.Omdb.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace Infrastructure;

public static class DependencyInjection
{
    private const int RetryCount = 3;
    private static readonly TimeSpan WaitBetweenRetry = TimeSpan.FromSeconds(2);

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        //register all IMovieSource implementations
        services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(classes => classes.AssignableTo<IMovieSource>())
            .As<IMovieSource>()
            .WithScopedLifetime()
        );

        services.ConfigureOmdbMovieSource(configuration);

        return services;
    }

    private static IServiceCollection ConfigureOmdbMovieSource(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.Configure<OmdbConfiguration>(configuration.GetSection(OmdbConfiguration.SectionName));

        // register HttpClient for OmdbMovieSource with RetryPolicy
        services.AddHttpClient(OmdbMovieSource.HttpClientName, (provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<OmdbConfiguration>>().Value
                              ?? throw new Exception($"{nameof(OmdbConfiguration)} doesn't exist");

                client.BaseAddress = options.BaseAddress
                                     ?? throw new Exception(
                                         $"{nameof(OmdbConfiguration)}:{nameof(options.BaseAddress)} should be set");
                client.Timeout = options.TimeOut;
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(RetryCount, _ => WaitBetweenRetry));

        return services;
    }
}