using Domain.ValueObjects;

namespace Domain.Providers;

public interface IMovieProvider
{
    Task<Movie?> FindMovie(string title);
}