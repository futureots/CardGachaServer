using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Database;

public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }
    
    public DbSet<Character> RegularCharacters { get; set; }
    public DbSet<CharacterPoolRelation>  CharacterPoolRelations { get; set; }
    public DbSet<Pool> Pools { get; set; }
    public DbSet<Rarity> Rarities { get; set; }
    
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.HasIndex(e => e.Name)
                .IsUnique();
            
            entity.HasOne(c => c.Rarity)
                .WithMany()
                .HasForeignKey(c => c.RarityId)
                .OnDelete(DeleteBehavior.Cascade);
        });
            
        modelBuilder.Entity<CharacterPoolRelation>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.Pool)
                .WithMany()
                .HasForeignKey(r => r.PoolId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Character)
                .WithMany()
                .HasForeignKey(r => r.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => new { r.PoolId, CharacterId = r.CharacterId }).IsUnique();

        });
        modelBuilder.Entity<Pool>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Rarity>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Item>()
            .HasKey(i => i.Id);

    }
}