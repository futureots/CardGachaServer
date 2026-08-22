namespace CardGachaServer.Model;

public class Rarity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name{get; set;} = string.Empty;
    
    public int Weight{get; set;}
}