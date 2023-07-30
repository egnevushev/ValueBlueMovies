using Domain.ValueObjects;

namespace Domain;

public interface IMovieCache
{
    Task<Movie?> GetOrCreate(string title, Func<Task<Movie?>> factory);
}