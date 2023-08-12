using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Movies;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extractors;

namespace WebApi.Controllers;

[ApiController, Route("api")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public MoviesController(IMoviesService moviesService) => _moviesService = moviesService;

    [HttpGet("movie")]
    [ProducesResponseType(typeof(Movie), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SearchMovie([FromQuery] string title, CancellationToken cancellationToken)
    {
        var ip = HttpContext.ExtractIpAddress();
        var request = new SearchRequest(title, ip, DateTime.Now);
        var movie = await _moviesService.SearchMovie(request, cancellationToken);
        
        return movie is null 
            ? new NotFoundResult()
            : new JsonResult(movie, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
}