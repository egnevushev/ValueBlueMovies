using Domain.ValueObjects;

namespace Domain.MovieSearchProviders;

public interface IMovieSearchProvider
{
    Task<Movie?> SearchMovie(string title);
}