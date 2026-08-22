using CardGachaServer.Database;
using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Service;

public interface IGachaService
{
    public Task<GachaResultDto?> GetRandomRegularCharacterAsync();
}
public class GachaService : IGachaService
{
    private readonly MasterDbContext _context;
    
    public GachaService(MasterDbContext context)
    {
        _context = context;
    }
    
    public async Task<GachaResultDto?> GetRandomRegularCharacterAsync()
    {
        var regularPoolQuery = await _context.RegularCharacters.AnyAsync(i => i.IsRegular);
        // 캐릭터가 하나도 존재하지 않을 경우(배포 기준 그럴일은 없음)
        if(!regularPoolQuery) return null;

        Console.WriteLine($"regularCharacter : {regularPoolQuery}");
        var existRarity = _context.Rarities
            .Where(r => _context.RegularCharacters.Any(c => c.RarityId == r.Id));
        
        // 희귀도 계산
        var rarity = await GetRandomRarityAsync(existRarity);
        // rarity 세팅이 안되어 있을 경우(배포 기준 그럴일은 없음)
        if(rarity == null) return null;
        var rarityList = _context.RegularCharacters
            .Where(c => c.RarityId == rarity.Id);
        var randomCharacter = await GetRandomCharacterAsync(rarityList);
        return new GachaResultDto(randomCharacter,rarity);
    }

    /// <summary>
    /// 쿼리 내부 동일한 확률로 1개 랜덤 선택해서 반환
    /// </summary>
    /// <param name="characters"></param>
    /// <returns></returns>
    async Task<Character?> GetRandomCharacterAsync(IQueryable<Character> characters)
    {
        var rarityList = await characters
            .ToListAsync();
        var rarity = rarityList.Count == 0 ? null : rarityList[Random.Shared.Next(rarityList.Count)];
        Console.WriteLine($"result : {rarity?.Name }");
        return rarity;
    }

    /// <summary>
    /// 쿼리 내부 레어도의 가중치에 기반하여 랜덤 레어도 1개 선택해서 반환
    /// </summary>
    /// <param name="rarities"></param>
    /// <returns></returns>
    async Task<Rarity?> GetRandomRarityAsync(IQueryable<Rarity> rarities)
    {
        var sum = await rarities.SumAsync(r => r.Weight);
        var rand = Random.Shared.Next(sum);
        var acc = 0;
        Console.WriteLine($"sum : {sum}, rand : {rand}");
        foreach (var item in rarities)
        {
            acc += item.Weight;
            if (rand >= acc) continue;
            Console.WriteLine($"result : {item.Name}");
            return item;
        }
        return null;
    }
}

public record GachaResultDto(Character? Character, Rarity Rarity);