using CardGachaServer.Model;
using Microsoft.EntityFrameworkCore;

namespace CardGachaServer.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemPoolRelation>  ItemPoolRelations { get; set; }
    public DbSet<Pool> Pools { get; set; }
    public DbSet<Probability> Probabilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasKey(i => i.Id);
        modelBuilder.Entity<ItemPoolRelation>(entity =>
        {
            entity.HasKey(r => r.Id);
            
            entity.HasOne(r => r.Pool)
                .WithMany()
                .HasForeignKey(r => r.PoolId);
            
            entity.HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId);

            entity.HasIndex(r => new { r.PoolId, r.ItemId }).IsUnique();

        });
        modelBuilder.Entity<Pool>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Probability>()
            .HasKey(p => p.Rarity);
            
    }
}