using Domain.ValueObjects;

namespace Domain.Providers;

public class CachedMovieProvider : IMovieProvider
{
    private readonly IMovieProvider _movieProvider;
    private readonly IMovieCacheStrategy _movieCacheStrategy;

    public CachedMovieProvider(IMovieProvider movieProvider, IMovieCacheStrategy movieCacheStrategy)
    {
        _movieProvider = movieProvider;
        _movieCacheStrategy = movieCacheStrategy;
    }

    public async Task<Movie?> FindMovie(string title) => 
        await _movieCacheStrategy.GetOrCreate(title, () => _movieProvider.FindMovie(title));
}