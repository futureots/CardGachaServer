namespace CardGachaServer.Model;

public class CharacterPoolRelation
{
    /// <summary>
    /// 고유 ID
    /// </summary>
    public Guid Id{get; set;} = Guid.NewGuid();
    
    /// <summary>
    /// 아이템 FK
    /// </summary>
    public Character? Character{get; set;}
    public string CharacterId { get; set; } = string.Empty;

    /// <summary>
    /// 풀 FK
    /// </summary>
    public Pool? Pool{get; set;}
    public string PoolId { get; set; } = string.Empty;
    
    
    
    
}