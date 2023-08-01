using Domain.Audit;
using Domain.Exceptions;
using Domain.MovieSearchProviders;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Movies;

public class MoviesService : IMoviesService
{
    private readonly IAuditService _auditService;
    private readonly IMovieSearchProvider _movieSearchProvider;
    private readonly ILogger<MoviesService> _logger;

    public MoviesService(IMovieSearchProvider movieSearchProvider, IAuditService auditService, ILogger<MoviesService> logger)
    {
        _auditService = auditService;
        _logger = logger;
        _movieSearchProvider = movieSearchProvider;
    }

    public async Task<Movie?> FindMovie(MovieRequest movieRequest, CancellationToken cancellationToken)
    {
        try
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
        catch (DomainException)
        {
            _logger.LogError("Exception has occured when processing {@Request}", movieRequest);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception has occured when processing request {@Request}", movieRequest);
            throw new DomainException("Exception has occured when processing request", exception);
        }
    }
}