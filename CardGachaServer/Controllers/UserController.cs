using System.Security.Claims;
using CardGachaServer.Model;
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

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn()
    {
        // TODO : 이거 나중에 튜토리얼 진행도 따로 저장해서 신규인지 기존 유저인지 확인하는 기능 추가하기
        var user = await _userService.GetOrCreateUser(User);
        // 이건 데이터가 이상한게 온거임.
        if(user == null) return BadRequest(new {message = "User not found"});
        
        return Ok(new {userId = user.FirebaseUid,  userName = user.Name});
    }

    [HttpPut("name")]
    public async Task<IActionResult> UpdateUserName([FromBody] UserName userName)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userId == null) return BadRequest(new {message = "Wrong user id"});
        
        var response = await _userService.UpdateUserNameAsync(userId, userName.Name);
        if(response == null) return BadRequest(new {message = "User not found"});
        
        return Ok(new {userId = response.FirebaseUid, userName = response.Name});
    }

    [HttpGet("character")]
    public async Task<IActionResult> GetUserCharacters()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrWhiteSpace(userId)) return Unauthorized(new {message = "Wrong user id"});
        var ownedCharacters = await _userService.GetUserCharacters(userId);
        return Ok(ownedCharacters);
    }
}
public record UserName(string Name);