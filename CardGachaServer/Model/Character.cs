namespace CardGachaServer.Model;

public class Character
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
    public string RarityId{get; set;}

    public Rarity? Rarity{get; set;}

    public bool IsRegular { get; set; } = false;
    
    // 이걸로 나중에 클라이언트 데이터랑 동기화 여부 확인하는 기능 추가해도 좋을듯
    //public DateTime LastModified { get; set; }
    // 캐릭터 기본 스탯이나 초기 마스터 데이터 저장 가능함.
    

}