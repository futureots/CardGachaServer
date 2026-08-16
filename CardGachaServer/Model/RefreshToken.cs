namespace CardGachaServer.Model;

/// <summary>
/// 나중에 Redis사용해서 캐시로 이전. 이전 후 필요없을 경우 제거할 예정
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsRevoked { get; set; } = false;
    public DateTime CreatedAt  { get; set; }
    public DateTime ExpiredAt  { get; set; }

}