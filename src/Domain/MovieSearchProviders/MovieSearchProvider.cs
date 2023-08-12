using Domain.Sources;
using Domain.ValueObjects;

namespace Domain.MovieSearchProviders;

public sealed class MovieSearchProvider : IMovieSearchProvider
{
    private readonly IEnumerable<IMovieSource> _movieSources;

    public MovieSearchProvider(IEnumerable<IMovieSource> movieSources)
    {
        _movieSources = movieSources;
    }

    public async Task<Movie?> SearchMovie(string title)
    {
        foreach (var source in _movieSources)
        {
            var movie = await source.FindMovie(title);
            if (movie is not null)
                return movie;
        }

        return null;
    }
}