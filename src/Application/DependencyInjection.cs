using Application.Admin;
using Application.Audit;
using Application.Movies;
using Domain.Audit;
using Domain.MovieSearchProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMoviesService, MoviesService>();
        services.AddScoped<IAdminService, AdminService>();
        
        services.AddScoped<IMovieSearchProvider, MovieSearchProvider>();
        services.Decorate<IMovieSearchProvider, CachedMovieSearchProvider>();
        
        services.AddScoped<IAuditService, AuditService>();
        
        return services;
    }
}