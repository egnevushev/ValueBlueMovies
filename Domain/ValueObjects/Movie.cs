namespace Domain.ValueObjects;

public sealed record Movie(
    string Title,
    string Year,
    string Rated,
    string Released,
    string Runtime,
    string Genre,
    string Director,
    string Writer,
    string Actors,
    string Plot,
    string Language,
    string Country,
    string Awards,
    string Poster,
    string ImdbRating,
    string ImdbVotes,
    string ImdbId
);