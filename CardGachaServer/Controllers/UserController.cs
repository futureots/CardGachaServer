using System.Security.Claims;
using CardGachaServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("character")]
    public async Task<IActionResult> GetUserCharacters()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrEmpty(userId)) return Unauthorized();
        var ownedCharacters = await _userService.GetUserCharacters(userId);
        return Ok(ownedCharacters);
    }
}