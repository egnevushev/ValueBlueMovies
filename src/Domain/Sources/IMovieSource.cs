using Domain.ValueObjects;

namespace Domain.Sources;

public interface IMovieSource
{
    Task<Movie?> FindMovie(string title);
}