using Domain.ValueObjects;

namespace Application.Movies;

public interface IMoviesService
{
    Task<Movie?> FindMovie(MovieRequest movieRequest, CancellationToken cancellationToken);
}