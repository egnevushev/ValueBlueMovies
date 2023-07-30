using Domain.Providers;
using Domain.ValueObjects;

namespace Application.Search;

public class SearchService : ISearchService
{
    //private readonly IAuditService _auditService;
    private readonly IMovieProvider _movieProvider;

    public SearchService(IMovieProvider movieProvider)
    {
        //_auditService = auditService;
        _movieProvider = movieProvider;
    }

    public async Task<Movie?> FindMovie(MovieRequest movieRequest)
    {
        var movie = await _movieProvider.FindMovie(movieRequest.Title);
        if (movie is null)
            return null;


        // crate RequestMeta
        
        //_auditService.LogAudit(RequestMeta);

        return movie;
    }
}