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
[ApiController, Route("api/audit")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService) => _adminService = adminService;

    [HttpGet("{Id}")]
    [ProducesResponseType(typeof(Audit), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FindById([FromQuery] FindByIdRequest request, CancellationToken token)
    {
        var audit = await _adminService.FindById(request.Id, token);
        return audit is null 
            ? new NotFoundResult()
            : new JsonResult(audit);
    }
    
    [HttpGet("all")]
    [ProducesResponseType(typeof(Audit[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRequest request, CancellationToken token)
    {
        var audits = await _adminService.GetAll(request.Count, request.LastId, token);
        return new JsonResult(audits);
    }
    
    [HttpGet("period/{start}/{end?}")]
    [ProducesResponseType(typeof(Audit[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DatePeriod([FromQuery] DatePeriodRequest request, CancellationToken token)
    {
        var audits = await _adminService.DatePeriod(request.Start, request.End,  request.Count, 
            request.LastId, token);
        return new JsonResult(audits);
    }
    
    [HttpGet("stat/daily/{start}/{end?}")]
    [ProducesResponseType(typeof(AuditStatPerDay[]), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetStatisticsPerDay([FromQuery] GetStatisticsPerDayRequest request, CancellationToken token)
    {
        var audits = await _adminService.GetStatisticsPerDay(request.Start, request.End, token);
        return new JsonResult(audits);
    }
    
    [HttpGet("stat/ip/{ip}")]
    [ProducesResponseType(typeof(IpAddressStat), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetStatisticsByIpAddress([FromQuery] GetStatisticsByIpAddressRequest request, CancellationToken token)
    {
        var audits = await _adminService.GetRequestsCountByIpAddress(request.IpAddress, token);
        return new JsonResult(audits);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Remove([FromQuery] RemoveByIdRequest request, CancellationToken token)
    {
        await _adminService.Remove(request.Id, token);
        return Ok();
    }
}