namespace Infrastructure.MovieSources.Omdb.Models;

public record OmdbResponse
(
    string? Title,
    string? Year,
    string? Rated,
    string? Released,
    string? Runtime,
    string? Genre,
    string? Director,
    string? Writer,
    string? Actors,
    string? Plot,
    string? Language,
    string? Country,
    string? Awards,
    string? Poster,
    string? ImdbRating,
    string? ImdbVotes,
    string? ImdbId,
    string? Response,
    string? Error
)
{
    public bool IsNotFoundResponse() => Error == OmdbConstants.NotFoundError;

    public bool IsErrorOccured(out string message)
    {
        message = string.Empty;
        if (Error != OmdbConstants.NotFoundError && Response == OmdbConstants.FalseResponse)
        {
            message = Response;
            return true;
        }

        return false;
    }
}