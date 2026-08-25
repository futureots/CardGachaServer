using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Database;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<OwnedCharacter> OwnedCharacters { get; set; }
    
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options){}

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        { 
            entity.HasKey(u => u.FirebaseUid);
        });

        modelBuilder.Entity<OwnedCharacter>(entity =>
        {
            entity.HasKey(o => new { o.UserId, o.CharacterId });
            
            entity.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}