using System.Threading;
using System.Threading.Tasks;
using Application.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
    public async Task<IActionResult> FindById([FromQuery] string id, CancellationToken cancellationToken)
    {
        var audit = await _adminService.FindById(id, cancellationToken);
        return new JsonResult(audit);
    }
    
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var audits = await _adminService.GetAll(cancellationToken);
        return new JsonResult(audits);
    }
    
    [HttpGet("remove")]
    public async Task<IActionResult> Remove([FromQuery] string id, CancellationToken cancellationToken)
    {
        await _adminService.Remove(id, cancellationToken);
        return Ok();
    }
}