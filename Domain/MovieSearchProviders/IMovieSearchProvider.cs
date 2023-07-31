using Domain.ValueObjects;

namespace Domain.MovieSearchProviders;

public interface IMovieSearchProvider
{
    Task<Movie?> FindMovie(string title);
}