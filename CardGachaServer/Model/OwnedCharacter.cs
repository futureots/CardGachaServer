namespace CardGachaServer.Model;

public class OwnedCharacter
{
    // 보유한 계정
    public string UserId { get; set; }
    public User? User { get; set; }
    // 보유한 캐릭터
    public string CharacterId { get; set; } = string.Empty;

    // TODO : 캐릭터가 가질 수 있는 값들(레벨, 돌파 횟수 등) 추가 필요
    public int Count { get; set; } = 1;
    public int Level { get; set; } = 1;
}