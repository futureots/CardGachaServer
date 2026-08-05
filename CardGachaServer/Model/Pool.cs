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
    
    /// <summary>
    /// 픽업이 나올 확률(나중에 천장 시스템이나 다른 시스템 추가시 변경이 필요함)
    /// </summary>
    public double FeatureRate{get; set;}
}