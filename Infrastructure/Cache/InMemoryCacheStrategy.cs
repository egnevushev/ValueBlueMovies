using System.Collections.Concurrent;
using Domain.MovieSearchProviders;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.Cache;

public class InMemoryCacheStrategy : IMovieCacheStrategy
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _expiration;

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ();

    public InMemoryCacheStrategy(IMemoryCache cache, IOptions<CacheConfiguration> options)
    {
        _cache = cache;
        _expiration = options.Value.Expiration;
    }

    public async Task<Movie?> GetOrCreate(string title, Func<Task<Movie?>> movieFactory)
    {
        if (_cache.TryGetValue(title, out Movie? movie)) 
            return movie;
        
        SemaphoreSlim titleLock = _locks.GetOrAdd(title, _ => new SemaphoreSlim(1, 1));
        await titleLock.WaitAsync();

        try
        {
            if (!_cache.TryGetValue(title, out movie))
            {
                // Key not in cache, so get data.
                movie = await movieFactory();
                
                var options = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetPriority(CacheItemPriority.High)
                    .SetAbsoluteExpiration(_expiration);
                
                _cache.Set(title, movie, options);
            }
        }
        finally
        {
            titleLock.Release();
        }

        return movie;
    }
}