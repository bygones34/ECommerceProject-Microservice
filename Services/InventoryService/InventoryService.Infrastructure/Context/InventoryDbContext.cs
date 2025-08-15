using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Context;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>(eb =>
        {
            eb.HasKey(i => i.Id);
            eb.Property(i => i.ProductId).IsRequired();
            eb.Property(i => i.AvailableQuantity).IsRequired();
            eb.HasIndex(i => i.ProductId).IsUnique();

            eb.Property<uint>("xmin")
             .HasColumnName("xmin")
             .IsConcurrencyToken()
             .ValueGeneratedOnAddOrUpdate();
        });
    }
}