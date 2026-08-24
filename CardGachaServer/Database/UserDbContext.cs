using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Database;

public class UserDbContext : DbContext
{
    public DbSet<OwnedCharacter> OwnedCharacters { get; set; }
    
    public DbSet<OwnedItem>  OwnedItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OwnedCharacter>()
            .HasKey(o => new { o.CharacterId, o.UserId });
        modelBuilder.Entity<OwnedItem>()
            .HasKey(o => new {o.UserId, o.ItemId});
    }
}