using System.Net;
using System.Net.Http.Json;
using System.Web;
using Domain.Sources;
using Domain.ValueObjects;
using Infrastructure.MovieSources.Omdb.Configuration;
using Infrastructure.MovieSources.Omdb.Models;
using MapsterMapper;
using Microsoft.Extensions.Options;

namespace Infrastructure.MovieSources.Omdb;

public class OmdbMovieSource : IMovieSource
{
    public const string HttpClientName = $"{nameof(OmdbMovieSource)}_httpClinet";
    
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IMapper _mapper;

    public OmdbMovieSource(IHttpClientFactory factory, IOptions<OmdbConfiguration> options, IMapper mapper)
    {
        _mapper = mapper;
        _httpClient = factory.CreateClient(HttpClientName);
        _apiKey = options.Value.ApiKey;
    }

    public async Task<Movie?> FindMovie(string title)
    {
        var response = await _httpClient.GetAsync($"/?t={HttpUtility.UrlEncode(title)}&apikey={_apiKey}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        
        var content = await response.Content.ReadFromJsonAsync<OmdbResponse>();
        if (content is null || content.IsNotFoundResponse())
            return null;

        if (content.IsErrorOccured(out var message))
            throw new Exception($"Omdb error: {message}");
        
        return _mapper.Map<Movie>(content);
    }
}