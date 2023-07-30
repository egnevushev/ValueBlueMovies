using Domain.ValueObjects;

namespace Domain.Providers;

public interface IMovieCacheStrategy
{
    Task<Movie?> GetOrCreate(string title, Func<Task<Movie?>> factory);
}