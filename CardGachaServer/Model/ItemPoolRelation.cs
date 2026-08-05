namespace CardGachaServer.Model;

public class ItemPoolRelation
{
    /// <summary>
    /// 고유 ID
    /// </summary>
    public Guid Id{get; set;} = Guid.NewGuid();
    
    /// <summary>
    /// 아이템 FK
    /// </summary>
    public Item? Item{get; set;}
    public string ItemId { get; set; } = string.Empty;

    /// <summary>
    /// 풀 FK
    /// </summary>
    public Pool? Pool{get; set;}
    public string PoolId { get; set; } = string.Empty;
    
    /// <summary>
    /// 해당 풀에서 해당 아이템의 픽업 여부
    /// </summary>
    public bool IsFeatured{get; set;}
    
    
    
}