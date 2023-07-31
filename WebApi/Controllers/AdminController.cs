using System.Threading;
using System.Threading.Tasks;
using Application.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Requests;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("audit")]
    public async Task<IActionResult> FindById([FromQuery] FindByIdRequest request, CancellationToken cancellationToken)
    {
        var audit = await _adminService.FindById(request.Id, cancellationToken);
        return new JsonResult(audit);
    }
    
    [HttpGet("audit/all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRequest request, CancellationToken cancellationToken)
    {
        var audits = await _adminService.GetAll(request.Count, request.LastId, cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpGet("audit/period")]
    public async Task<IActionResult> DatePeriod([FromQuery] DatePeriodRequest request, CancellationToken cancellationToken)
    {
        var audits = await _adminService.DatePeriod(request.Start, request.End,  request.Count, 
            request.LastId, cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpGet("audit/stat")]
    public async Task<IActionResult> GetStatisticsPerDay(CancellationToken cancellationToken)
    {
        var audits = await _adminService.GetStatisticsPerDay(cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpDelete("audit")]
    public async Task<IActionResult> Remove([FromQuery] string id, CancellationToken cancellationToken)
    {
        await _adminService.Remove(id, cancellationToken);
        return Ok();
    }
}