using CardGachaServer.Database;
using CardGachaServer.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GachaController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public GachaController(ApplicationDbContext context) : base()
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Gacha([FromBody] GachaData gachaData)
    {
        // 상시 아이템이 있는지 확인
        var regularPoolQuery = await _context.Items.AnyAsync(i => i.IsRegular);
        if(!regularPoolQuery) return NotFound();

        List<ItemData> result = new();
        for (var i = 0; i < gachaData.Count; i++)
        {
            var singleResult = await GetRandomItem();
            if (singleResult == null) return NotFound();
            result.Add(singleResult);
        }
        return Ok(result);
    }
    
    [HttpPost("{poolId}")]
    public async Task<IActionResult> Gacha(string poolId, [FromBody]GachaData data)
    {
        // 뽑기 풀이 존재하는지 여부를 확인
        var pool = await _context.Pools.FindAsync(poolId);
        // 풀 자체가 없어서 실패하는 경우
        if (pool == null) return NotFound();
        
        List<ItemData> gachaResult = new();
        // 뽑기 n회 진행
        for (int i = 0; i < data.Count; i++)
        {
            var result = await GetRandomItem(pool);
            // 해당 풀 테이블에 데이터가 없어서 실패하는 경우
            if (result == null) return NotFound();
            gachaResult.Add(result);
        }
        return  Ok(gachaResult);
    }

    /// <summary>
    /// 특정 풀 내부의 뽑기 1회 진행
    /// </summary>
    /// <param name="pool"></param>
    /// <returns></returns>
    async Task<ItemData?> GetRandomItem(Pool pool)
    {
        var poolQuery = _context.ItemPoolRelations
            .Where(r => r.PoolId == pool.Id);

        // 풀이 있는지 확인
        if (!poolQuery.Any()) return null;
        
        // 희귀도 계산
        var sum = await _context.Probabilities.SumAsync(p => p.Weight);
        var rand = Random.Shared.Next(sum);
        var acc = 0;
        var rarity = 0;
        foreach (var item in _context.Probabilities)
        {
            acc += item.Weight;
            if (rand >= acc) continue;
            rarity = item.Rarity;
            break;
        }
        
        // 데이터 베이스에서 희귀도 쿼리한 리스트 반환
        var poolList = await poolQuery
            .Include(r => r.Item)
            .Where(r => r.Item != null && r.Item.Rarity == rarity)
            .ToListAsync();
        // 동일 희귀도 및 픽업 여부 리스트에서 랜덤한 값 1개 추출(없을 경우 에러 반환 => 기본값 반환?)
        var o = poolList[Random.Shared.Next(poolList.Count)].Item;
        if (o == null) return null;
        return new ItemData(o.Id, o.Name);
    }

    async Task<ItemData?> GetRandomItem()
    {
        var regularPoolQuery = _context.Items
            .Where(i => i.IsRegular);
        
        if (!regularPoolQuery.Any()) return null;

        var prob = regularPoolQuery
            .GroupBy(i => i.Rarity)
            .Select(g => new { Rarity = g.Key, Count = g.Count() })
            .Join(
            _context.Probabilities,
            i => i.Rarity,
            p => p.Rarity,
            (i, p) => new
            {
                p.Weight,
                p.Rarity,
                Count = i.Count
            });
        // 희귀도 계산
        var sum = await prob.SumAsync(i => i.Weight);
        var rand = Random.Shared.Next(sum);
        var acc = 0;
        var rarity = 0;
        foreach (var item in _context.Probabilities)
        {
            acc += item.Weight;
            if (rand >= acc) continue;
            rarity = item.Rarity;
            break;
        }
        
        // 데이터 베이스에서 희귀도 쿼리한 리스트 반환
        var poolList = await regularPoolQuery
            .Where(i => i.Rarity == rarity)
            .ToListAsync();
        // 동일 희귀도 및 픽업 여부 리스트에서 랜덤한 값 1개 추출(없을 경우 에러 반환 => 기본값 반환?)
        var result = poolList[Random.Shared.Next(poolList.Count)];
        return  new ItemData(result.Id,result.Name);
    }
}

public record TableData(int Rarity, string Id);

public record GachaData(int Count);

public record ItemData(string Id, string Name);

