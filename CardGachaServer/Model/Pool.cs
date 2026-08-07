namespace CardGachaServer.Model;

public class Pool
{
    public string Id{get; set;} = Guid.NewGuid().ToString();
    
    /// <summary>
    /// 시작 시간
    /// </summary>
    public DateTime ValidateDate{get; set;}
    
    /// <summary>
    /// 만료 시간
    /// </summary>
    public DateTime ExpireDate{get; set;}
}