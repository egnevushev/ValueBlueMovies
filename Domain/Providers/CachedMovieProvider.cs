using Domain.ValueObjects;

namespace Domain.Providers;

public class CachedMovieProvider : IMovieProvider
{
    private readonly IMovieProvider _movieProvider;

    public CachedMovieProvider(IMovieProvider movieProvider)
    {
        _movieProvider = movieProvider;
    }

    public Task<Movie?> FindMovie(string title)
    {
        //check in cache
        return _movieProvider.FindMovie(title);
    }
}