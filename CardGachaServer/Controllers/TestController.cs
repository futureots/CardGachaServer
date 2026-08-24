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
    private readonly MasterDbContext _context;
    
    public TestController(MasterDbContext context) : base()
    {
        _context = context;
    }

    [HttpPost("character")]
    public async Task<IActionResult> PostCharacterData([FromBody] CharacterDto charData)
    {
        var rarity = await _context.Rarities.FirstOrDefaultAsync(r => r.Name == charData.Rarity);
        if (rarity == null) return NotFound(new {message = "Rarity not found"});
        
        var character = new Character()
        {
            Rarity = rarity,
            Name = charData.CharacterName,
            IsRegular = true,
        };
        await _context.RegularCharacters.AddAsync(character);
        await _context.SaveChangesAsync();
        
        return Ok(character);
    }

    [HttpPost("rarity")]
    public async Task<IActionResult> PostRarityData([FromBody] RarityData rarityData)
    {
        var rarity = await _context.Rarities.FirstOrDefaultAsync(r => r.Name == rarityData.Rarity);
        if (rarity == null)
        {
            rarity = new Rarity()
            {
                Name = rarityData.Rarity,
                Weight = rarityData.Weight
            };
            _context.Rarities.Add(rarity);
        }
        else
        {
            rarity.Weight = rarityData.Weight;
        }
        await _context.SaveChangesAsync();
        return Ok(rarity);
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