using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPlayerData()
    {
        // 계정 id 가져와서 db에서 해당 계정 리스트 반환하기
        return Ok();
    }
}