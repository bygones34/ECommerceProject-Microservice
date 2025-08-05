using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductId)
                .IsRequired();

            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(oi => oi.Quantity)
                .IsRequired();
        }
    }
}