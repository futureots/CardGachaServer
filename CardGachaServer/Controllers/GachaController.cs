using System.Security.Claims;
using CardGachaServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GachaController : ControllerBase
{
    private readonly IGachaService _gachaService;
    private readonly IUserService _userService;
    public GachaController(IGachaService gachaService, IUserService userService) : base()
    {
        _gachaService = gachaService;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Gacha([FromBody] GachaRequest gachaRequest)
    {
        List<CharacterResponse> response = new();
        for (var i = 0; i < gachaRequest.Count; i++)
        {
            // 캐릭터 랜덤 뽑기
            var result = await _gachaService.GetRandomRegularCharacterAsync();
            if (result?.Character == null) return NotFound(new {message = "Character not found"});
            
            // 캐릭터를 계정에 추가하기(중복 결과 반환)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 그럴리 없지만 일단 해놓기
            if(string.IsNullOrWhiteSpace(userId)) return Unauthorized(new {message = "Wrong user id"});
            
            var isFirst = await _userService.AddOwnedCharacter(userId,result.Character);
            
            // TODO : 나중엔 name이랑 rarity이름을 보낼 필요 없이 id만 전송하고, 클라에서 id로 매핑된 값 사용하기
            var data = new CharacterResponse(
                Id: result.Character.Id,
                Name: result.Character.Name,
                RarityId: result.Rarity.Id,
                Rarity: result.Rarity.Name,
                IsFirst: isFirst
            );
            response.Add(data);
        }
        return Ok(response);
        // 클라이언트에서 false가 하나라도 있으면 인벤토리 데이터를 업데이트 하도록 구현하기?
    }
    
}

public record GachaRequest(int Count);

public record CharacterResponse(string Id, string Name, string RarityId,string Rarity, bool IsFirst);

