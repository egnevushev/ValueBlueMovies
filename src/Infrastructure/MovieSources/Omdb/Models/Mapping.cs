using Domain.ValueObjects;
using Mapster;

namespace Infrastructure.MovieSources.Omdb.Models;

public sealed class Mapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OmdbResponse, Movie>();
    }
}