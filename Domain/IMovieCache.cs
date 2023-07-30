using Domain.ValueObjects;

namespace Domain;

public interface IMovieCache
{
    Task<Movie?> GetOrCreate(string title, Func<Task<Movie?>> factory);
    
    Task<Movie?> Find(string title);
    
    Task Set(string title, Movie movie);
}