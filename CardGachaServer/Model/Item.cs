namespace CardGachaServer.Model;

public class Item
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Name { get; set; } =  string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    
    // 카테고리 값을 추가해서 기능은 클라이언트에서, 종류는 서버에서 진행해도 될듯.
    //public DateTime LastModified { get; set; } = DateTime.UtcNow;
    
}