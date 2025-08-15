using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Context
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var p = modelBuilder.Entity<Payment>();
            p.HasKey(x => x.Id);

            p.Property(x => x.OrderId).IsRequired();
            p.Property(x => x.UserName).HasMaxLength(200).IsRequired();
            p.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();
            p.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            p.Property(x => x.ProviderReference).HasMaxLength(100);
            p.Property(x => x.CreatedAt).IsRequired();
            p.Property(x => x.UpdatedAt).IsRequired();

            p.Property<uint>("xmin")
             .HasColumnName("xmin")
             .IsConcurrencyToken()
             .ValueGeneratedOnAddOrUpdate();
            p.HasIndex(x => x.OrderId).IsUnique();
        }
    }
}
