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

    [HttpGet("findById")]
    public async Task<IActionResult> FindById([FromQuery] FindByIdRequest request, CancellationToken cancellationToken)
    {
        var audit = await _adminService.FindById(request.Id, cancellationToken);
        return new JsonResult(audit);
    }
    
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRequest request, CancellationToken cancellationToken)
    {
        var audits = await _adminService.GetAll(request.Count, request.LastId, cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpGet("remove")]
    public async Task<IActionResult> Remove([FromQuery] string id, CancellationToken cancellationToken)
    {
        await _adminService.Remove(id, cancellationToken);
        return Ok();
    }
}