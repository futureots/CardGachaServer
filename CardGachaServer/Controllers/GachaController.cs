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
    
    // TODO : 나중에 db에서 id를 사용해서 해당 테이블 불러오는 방식 사용
    private List<(int, string)> _itemTable =
    [
        (3, "ball"),
        (7, "tissue")
    ];
    
    [HttpGet("{poolId}")]
    public async Task<IActionResult> GetItemTable(string poolId)
    {
        var pool = await _context.Pools.FindAsync(poolId);
        if(pool == null) return NotFound();

        // id rarity rate
        var list = new List<TableData>();

        var poolQuery = _context.ItemPoolRelations
            .Where(r => r.PoolId == pool.Id);
        
        var probabilities = await poolQuery
            .Select(r => new
            {
                Rarity = r.Item!.Rarity,
                ItemId = r.ItemId
            })
            .GroupBy(r => r.Rarity)
            .Select(g => new
            {
                Rarity = g.Key,
                Count = g.Count() // 그룹별 개수 집계
            })
            .Join(
                _context.Probabilities,
                g => g.Rarity,
                p => p.Rarity,
                (g, p) => new
                {
                    Rarity = g.Rarity,
                    Count = g.Count, //count는 항상 0보다 큼
                    Weight = p.Weight
                }
            )
            .ToListAsync();
        
        var sum = probabilities.Sum(r => r.Weight);
        foreach (var probability in probabilities)
        {
            // 동일 희귀도 내의 아이템 리스트 반환
            var rarityList = await poolQuery
                .Select(r => r.Item)
                .Where(r => r.Rarity == probability.Rarity)
                .ToListAsync();
        }
        // var sum = _itemTable.Sum(item => item.Item1);
        // var data = _itemTable.Select(item =>
        // {
        //     var (item1, item2) = item;
        //     return new TableData($"{(double)item1 / sum * 100}%", $"{item2}");
        // }).ToList();
        return Ok(list);
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
        var result = o.Name;
        return  new ItemData(result);

    }
}

public record TableData(string Rate, string Id);

public record GachaData(int Count);

public record ItemData(string Id);

