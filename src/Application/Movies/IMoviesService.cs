using Domain.ValueObjects;

namespace Application.Movies;

public interface IMoviesService
{
    Task<Movie?> SearchMovie(SearchRequest request, CancellationToken cancellationToken);
}