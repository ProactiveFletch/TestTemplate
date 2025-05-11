using Microsoft.AspNetCore.Mvc;
using SoftwareVersioner.Core;
using SoftwareVersioner.Core.Interfaces;

namespace SoftwareVersioner.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SoftwareController : ControllerBase
{
    private readonly ISoftwareManager _svc;
    private readonly ILogger<SoftwareController> _logger;

    public SoftwareController(ISoftwareManager svc, ILogger<SoftwareController> logger)
    {
        _svc = svc;
        _logger = logger;
    }

    [HttpGet("all")] // Changed route to reflect its purpose
    public ActionResult<IEnumerable<Software>> GetAllSoftware()
    {
        _logger.LogInformation("Fetching all software from SoftwareManager.");
        try
        {
            var allSoftware = _svc.GetAllSoftware();
            return Ok(allSoftware);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error retrieving software from SoftwareManager.");
            return StatusCode(500, "An unexpected error occurred while retrieving software list.");
        }
    }
}