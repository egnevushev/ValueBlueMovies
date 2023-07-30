using Application.Admin;
using Application.Audit;
using Application.Search;
using Domain.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IAdminService, AdminService>();
        
        services.AddScoped<IMovieProvider, MovieProvider>();
        services.Decorate<IMovieProvider, CachedMovieProvider>();
        
        //services.AddScoped<IAuditService, AuditService>();
        
        return services;
    }
}