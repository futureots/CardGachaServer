using Microsoft.AspNetCore.Mvc;

namespace CardGachaServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GachaController : ControllerBase
{
    
    // TODO : 나중에 db에서 id를 사용해서 해당 테이블 불러오는 방식 사용
    private List<(int, string)> _itemTable =
    [
        (3, "ball"),
        (7, "tissue")
    ];
    
    [HttpGet("{tableId}")]
    public Task<IActionResult> GetItemTable(string tableId)
    {
        var sum = _itemTable.Sum(item => item.Item1);
        var data = _itemTable.Select(item =>
        {
            var (item1, item2) = item;
            return new TableData($"{(double)item1 / sum * 100}%", $"{item2}");
        }).ToList();
        return Task.FromResult<IActionResult>(Ok(data));
    }

    [HttpPost("{tableId}")]
    public Task<IActionResult> Gacha(string tableId, [FromBody]GachaData data)
    {
        List<ItemData> gachaResult = new();
        for (int i = 0; i < data.Count; i++)
        {
            gachaResult.Add(GetRandomItem());
        }
        return  Task.FromResult<IActionResult>(Ok(gachaResult));
    }

    ItemData GetRandomItem()
    {
        var rand = Random.Shared;
        var sum = _itemTable.Sum(item => item.Item1);
        var roll = rand.Next(0, sum);

        var acc = 0;
        foreach (var item in _itemTable)
        {
            acc += item.Item1;
            if (roll < acc)
            {
                return new ItemData(item.Item2);
            }
        }
        
        // 애초에 이게 나오면 문제가 발생한 거임.
        return new ItemData(_itemTable.Last().Item2);
    }
}

public record TableData(string Rate, string Id);

public record GachaData(int Count);

public record ItemData(string Id);

