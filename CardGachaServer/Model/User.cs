namespace CardGachaServer.Model;

public class User
{
    /// <summary>
    /// 고유 id pk(firebase에서는 fk)
    /// </summary>
    public required string FirebaseUid { get; set; }
    public bool IsBanned { get; set; }
    
    public string Name  { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    
    
}