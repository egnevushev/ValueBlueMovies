using Domain.ValueObjects;

namespace Domain.MovieSearchProviders;

public interface IMovieCacheStrategy
{
    Task<Movie?> GetOrCreate(string title, Func<Task<Movie?>> factory);
}