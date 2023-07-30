using Domain.Sources;
using Domain.ValueObjects;

namespace Domain.Providers;

public class MovieProvider : IMovieProvider
{
    private readonly IEnumerable<IMovieSource> _movieSources;

    public MovieProvider(IEnumerable<IMovieSource> movieSources)
    {
        _movieSources = movieSources;
    }

    public async Task<Movie?> FindMovie(string title)
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