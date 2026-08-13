using CardGachaServer.Service;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

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
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshToken)
    {
        var result = await _authService.Refresh(refreshToken.RefreshToken);
        if(result== null) return  Unauthorized(new {error = "유효하지 않은 토큰입니다."});
        return Ok(result);
    }
}

public record GoogleLoginRequest(string IdToken);