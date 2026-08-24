using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Database;

public class UserDbContext : DbContext
{
    public DbSet<OwnedCharacter> OwnedCharacters { get; set; }
    
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options){}

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OwnedCharacter>()
            .HasKey(o => new { o.CharacterId, o.UserId });
    }
}