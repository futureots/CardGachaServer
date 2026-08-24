namespace CardGachaServer.Model;

public class User
{
    public string Id { get; set; } =  Guid.NewGuid().ToString();
    
    // 인게임에서 표현될 플레이어 닉네임
    public string Name { get; set; }
    
    // 계정 종류
    public string Provider { get; set; }
    
    // 계정 sub
    public string ProviderId { get; set; }
    
}