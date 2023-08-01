using Domain.ValueObjects;

namespace Domain.MovieSearchProviders;

public class CachedMovieSearchProvider : IMovieSearchProvider
{
    private readonly IMovieSearchProvider _movieSearchProvider;
    private readonly IMovieCacheStrategy _movieCacheStrategy;

    public CachedMovieSearchProvider(IMovieSearchProvider movieSearchProvider, IMovieCacheStrategy movieCacheStrategy)
    {
        _movieSearchProvider = movieSearchProvider;
        _movieCacheStrategy = movieCacheStrategy;
    }

    public async Task<Movie?> FindMovie(string title) => 
        await _movieCacheStrategy.GetOrCreate(title, () => _movieSearchProvider.FindMovie(title));
}