using CardGachaServer.Database;
using CardGachaServer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly MasterDbContext _masterContext;
    
    public TestController(MasterDbContext masterContext) : base()
    {
        _masterContext = masterContext;
    }

    [HttpPost("character")]
    public async Task<IActionResult> PostCharacterData([FromBody] CharacterDto charData)
    {
        var rarity = await _masterContext.Rarities.FirstOrDefaultAsync(r => r.Name == charData.Rarity);
        if (rarity == null) return NotFound(new {message = "Rarity not found"});
        
        var character = new Character()
        {
            Rarity = rarity,
            Name = charData.CharacterName,
            IsRegular = true,
        };
        await _masterContext.RegularCharacters.AddAsync(character);
        await _masterContext.SaveChangesAsync();
        
        return Ok(character);
    }

    [HttpPost("rarity")]
    public async Task<IActionResult> PostRarityData([FromBody] RarityData rarityData)
    {
        var rarity = await _masterContext.Rarities.FirstOrDefaultAsync(r => r.Name == rarityData.Rarity);
        if (rarity == null)
        {
            rarity = new Rarity()
            {
                Name = rarityData.Rarity,
                Weight = rarityData.Weight
            };
            _masterContext.Rarities.Add(rarity);
        }
        else
        {
            rarity.Weight = rarityData.Weight;
        }
        await _masterContext.SaveChangesAsync();
        return Ok(rarity);
    }

    [HttpGet("character")]
    public async Task<IActionResult> GetCharacterTable()
    {
        var result = await _masterContext.RegularCharacters
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("rarity")]
    public async Task<IActionResult> GetRarityTable()
    {
        var result = await _masterContext.Rarities
            .ToListAsync();
        return Ok(result);
    }
    [Authorize]
    [HttpPost("echo")]
    public Task<IActionResult> Echo([FromBody] EchoData data)
    {
        return Task.FromResult<IActionResult>(Ok(data));
    }
}

public record EchoData(string Message);

public record CharacterDto(string CharacterName, string Rarity);

public record RarityData(string Rarity,int Weight);