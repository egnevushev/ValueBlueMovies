using Domain.MovieSearchProviders;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.Cache;

public class InMemoryCacheStrategy : IMovieCacheStrategy
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _expiration;

    public InMemoryCacheStrategy(IMemoryCache cache, IOptions<CacheConfiguration> options)
    {
        _cache = cache;
        _expiration = options.Value.Expiration;
    }

    public async Task<Movie?> GetOrCreate(string title, Func<Task<Movie?>> movieFactory)
    {
        if (_cache.TryGetValue(title, out Movie? movie))
            return movie;

        movie = await movieFactory();

        var options = new MemoryCacheEntryOptions()
            .SetSize(1)
            .SetPriority(CacheItemPriority.High)
            .SetAbsoluteExpiration(_expiration);

        _cache.Set(title, movie, options);

        return movie;
    }
}