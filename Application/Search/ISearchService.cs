using Domain.ValueObjects;

namespace Application.Search;

public interface ISearchService
{
    Task<Movie?> FindMovie(MovieRequest movieRequest);
}