using CardGachaServer.Service;
using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService  _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login/google")]
    public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest idToken)
    {
        var result = await _authService.LoginGoogle(idToken.IdToken);
        if(result == null) return Unauthorized(new {error = "유효하지 않은 로그인 방식입니다."});
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var result = await _authService.RefreshAsync(req.RefreshToken);
        if (result == null)
        {
            return  Unauthorized(new {error = "유효하지 않은 토큰입니다."});
        }
        return Ok(result);
    }
}

public record GoogleLoginRequest(string IdToken);

public record RefreshRequest(string RefreshToken);