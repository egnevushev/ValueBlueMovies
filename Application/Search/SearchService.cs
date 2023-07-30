using Application.Audit;
using Domain.Providers;
using Domain.ValueObjects;

namespace Application.Search;

public class SearchService : ISearchService
{
    private readonly IAuditService _auditService;
    private readonly IMovieProvider _movieProvider;

    public SearchService(IMovieProvider movieProvider, IAuditService auditService)
    {
        _auditService = auditService;
        _movieProvider = movieProvider;
    }

    public async Task<Movie?> FindMovie(MovieRequest movieRequest)
    {
        var movie = await _movieProvider.FindMovie(movieRequest.Title);
        if (movie is null)
            return null;

        RequestMeta requestMeta = new(
            movieRequest.Title,
            movie.ImdbId,
            movieRequest.Requested,
            DateTime.Now,
            movieRequest.IpAddress);
        
        _auditService.AuditRequest(requestMeta);

        return movie;
    }
}