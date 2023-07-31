using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Movies;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesService _moviesService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(ILogger<MoviesController> logger, IMoviesService moviesService)
    {
        _logger = logger;
        _moviesService = moviesService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title, CancellationToken cancellationToken)
    {
        var request = new MovieRequest(title, HttpContext.Connection.RemoteIpAddress, DateTime.Now);
        var movie = await _moviesService.FindMovie(request, cancellationToken);
        return new JsonResult(movie);
    }

}