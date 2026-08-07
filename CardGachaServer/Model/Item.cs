namespace CardGachaServer.Model;

public class Item
{
    /// <summary>
    /// DB에서 사용할 아이템 식별자
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 클라이언트로 반환될 아이템 식별자
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 아이템 희귀도
    /// </summary>
    public int Rarity{get; set;}

    public Probability? Probability{get; set;}

}