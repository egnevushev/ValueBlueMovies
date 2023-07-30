using Domain.ValueObjects;

namespace Domain.Providers;

public class CachedMovieProvider : IMovieProvider
{
    private readonly IMovieProvider _movieProvider;
    private readonly IMovieCache _movieCache;

    public CachedMovieProvider(IMovieProvider movieProvider, IMovieCache movieCache)
    {
        _movieProvider = movieProvider;
        _movieCache = movieCache;
    }

    public async Task<Movie?> FindMovie(string title) => 
        await _movieCache.GetOrCreate(title, () => _movieProvider.FindMovie(title));
}