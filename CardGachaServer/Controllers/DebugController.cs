using CardGachaServer.Database;
using CardGachaServer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public DebugController(ApplicationDbContext context) : base()
    {
        _context = context;
    }
    [HttpPost]
    public async Task<IActionResult> PostTestData([FromBody] DataDto itemData)
    {
        var item = new Item()
        {
            Rarity = itemData.Rarity,
            Name = itemData.ItemName,
            IsRegular = true,
        };
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        
        return Ok(item);
    }

    [HttpPost("percent")]
    public async Task<IActionResult> PostTest([FromBody] ProbabilityDto probData)
    {
        var prob = await _context.Probabilities.FindAsync(probData.Rarity);
        if (prob == null)
        {
            prob = new Probability()
            {
                Rarity = probData.Rarity,
                Weight = probData.Weight
            };
            await _context.Probabilities.AddAsync(prob);
        }
        else
        {
            prob.Weight = probData.Weight;
        }
        await _context.SaveChangesAsync();
        return Ok(prob);
    }

    [HttpGet]
    public async Task<IActionResult> GetItemTable()
    {
        var result = await _context.Items.ToListAsync();
        return Ok(result);
    }
}

public record DataDto(string ItemName, int Rarity);

public record ProbabilityDto(int Rarity,int Weight);