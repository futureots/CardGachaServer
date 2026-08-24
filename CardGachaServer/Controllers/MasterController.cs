using CardGachaServer.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MasterController : ControllerBase
{
    private readonly MasterDbContext _masterContext;
    
    public MasterController(MasterDbContext masterContext)
    {
        _masterContext = masterContext;
    }
    
    [HttpGet("character")]
    public async Task<IActionResult> GetCharacterTable()
    {
        var result = await _masterContext.RegularCharacters
            .Select(c =>new 
            {
                c.Id,
                c.Name,
                c.RarityId
                // TODO : 나중에 여기에 추가적으로 초기 스탯들 부여하기
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("rarity")]
    public async Task<IActionResult> GetRarityTable()
    {
        var result = await _masterContext.Rarities
            .Select(r => new
            {
                r.Id,
                r.Name
            })
            .ToListAsync();
        return Ok(result);
    }

}