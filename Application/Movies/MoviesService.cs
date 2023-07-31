using Domain.Audit;
using Domain.MovieSearchProviders;
using Domain.ValueObjects;

namespace Application.Movies;

public class MoviesService : IMoviesService
{
    private readonly IAuditService _auditService;
    private readonly IMovieSearchProvider _movieSearchProvider;

    public MoviesService(IMovieSearchProvider movieSearchProvider, IAuditService auditService)
    {
        _auditService = auditService;
        _movieSearchProvider = movieSearchProvider;
    }

    public async Task<Movie?> FindMovie(MovieRequest movieRequest, CancellationToken cancellationToken)
    {
        var movie = await _movieSearchProvider.FindMovie(movieRequest.Title);
        if (movie is null)
            return null;

        RequestMeta requestMeta = new(
            movieRequest.Title,
            movie.ImdbId,
            movieRequest.Requested,
            DateTime.Now,
            movieRequest.IpAddress);
        
        _auditService.AuditRequest(requestMeta, cancellationToken);

        return movie;
    }
}