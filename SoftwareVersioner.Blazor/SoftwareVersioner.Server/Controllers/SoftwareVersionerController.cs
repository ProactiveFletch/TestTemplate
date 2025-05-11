using Microsoft.AspNetCore.Mvc;

namespace SoftwareVersioner.Server.Controllers;

public class NameController : ControllerBase
{
    [HttpPost("api/Name")]
    public IActionResult ProcessName([FromBody] string? nameInput)
    {
        var name = string.IsNullOrWhiteSpace(nameInput) ? "WORLD" : nameInput.ToUpper();
        return Ok(name);
    }
}
