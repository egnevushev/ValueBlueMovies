using System;
using System.Threading.Tasks;
using Application.Search;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ILogger<SearchController> logger, ISearchService searchService)
    {
        _logger = logger;
        _searchService = searchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string title)
    {
        var request = new MovieRequest(title, HttpContext.Connection.RemoteIpAddress, DateTime.Now);
        var movie = await _searchService.FindMovie(request);
        return new JsonResult(movie);
    }

}