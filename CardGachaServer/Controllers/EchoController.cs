using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EchoController : ControllerBase
{
    [HttpPost]
    public Task<IActionResult> Echo([FromBody] EchoData data)
    {
        return Task.FromResult<IActionResult>(Ok(data));
    } 
}

public record EchoData(string Message);