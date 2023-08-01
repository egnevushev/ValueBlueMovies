using Domain.ValueObjects;
using Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureUnitTests;

public class InMemoryCacheStrategyTests
{
    [Fact]
    public async Task GetOrCreate_CacheHit_ReturnsCachedMovie()
    {
        var movieTitle = "Test Movie";
        object expectedMovie = CreateMovie(movieTitle);
        var memoryCacheMock = new Mock<IMemoryCache>();
        
        memoryCacheMock
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedMovie))
            .Returns(true);
        var cacheStrategy = new InMemoryCacheStrategy(memoryCacheMock.Object, CreateOptions());

        // Act
        var actualMovie = await cacheStrategy.GetOrCreate(movieTitle, () => Task.FromResult<Movie?>(null));

        // Assert
        Assert.Equal(expectedMovie, actualMovie);
    }

    [Fact]
    public async Task GetOrCreate_CacheMiss_ReturnsNewMovie()
    {
        // Arrange
        var movieTitle = "Test Movie";
        var movieFactoryInvoked = false;
        var memoryCacheMock = new Mock<IMemoryCache>();
        memoryCacheMock
            .Setup(cache => cache.TryGetValue(movieTitle, out It.Ref<object>.IsAny))
            .Returns(false);
        var cacheEntryMock = new Mock<ICacheEntry>();
        memoryCacheMock
            .Setup(cache => cache.CreateEntry(movieTitle))
            .Returns(cacheEntryMock.Object);

        var cacheStrategy = new InMemoryCacheStrategy(memoryCacheMock.Object, CreateOptions());

        // Act
        var actualMovie = await cacheStrategy.GetOrCreate(movieTitle, () =>
        {
            movieFactoryInvoked = true;
            return Task.FromResult<Movie?>(CreateMovie(movieTitle));
        });

        // Assert
        Assert.NotNull(actualMovie);
        Assert.True(movieFactoryInvoked);
    }

    private static Movie CreateMovie(string title) => new Movie(title, "", "", "",
        "", "", "", "", "", "", "", "",
        "", "", "", "", "");
    
    private static IOptions<CacheConfiguration> CreateOptions() => 
        Options.Create(new CacheConfiguration { Expiration = TimeSpan.FromMinutes(30) });
}