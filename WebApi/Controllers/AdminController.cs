using System.Threading;
using System.Threading.Tasks;
using Application.Admin;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication;
using WebApi.Requests;

namespace WebApi.Controllers;

[ApiKeyAuthFilter]
[ApiController, Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService) => _adminService = adminService;

    [HttpGet("audit")]
    [ProducesResponseType(typeof(Audit), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FindById([FromQuery] FindByIdRequest request, CancellationToken cancellationToken)
    {
        var audit = await _adminService.FindById(request.Id, cancellationToken);
        return audit is null 
            ? new NotFoundResult()
            : new JsonResult(audit);
    }
    
    [HttpGet("audit/all")]
    [ProducesResponseType(typeof(Audit[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRequest request, CancellationToken cancellationToken)
    {
        var audits = await _adminService.GetAll(request.Count, request.LastId, cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpGet("audit/period")]
    [ProducesResponseType(typeof(Audit[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DatePeriod([FromQuery] DatePeriodRequest request, CancellationToken cancellationToken)
    {
        var audits = await _adminService.DatePeriod(request.Start, request.End,  request.Count, 
            request.LastId, cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpGet("audit/stat")]
    [ProducesResponseType(typeof(AuditStatPerDay[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetStatisticsPerDay(CancellationToken cancellationToken)
    {
        var audits = await _adminService.GetStatisticsPerDay(cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpDelete("audit")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Remove([FromQuery] string id, CancellationToken cancellationToken)
    {
        await _adminService.Remove(id, cancellationToken);
        return Ok();
    }
}