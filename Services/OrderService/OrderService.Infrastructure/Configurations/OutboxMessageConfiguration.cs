using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Outbox;

namespace OrderService.Infrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> b)
        {
            b.ToTable("OutboxMessages");
            b.HasKey(x => x.Id);

            b.Property(x => x.TypeName).HasMaxLength(500).IsRequired();
            b.Property(x => x.Payload).IsRequired();
            b.Property(x => x.OccurredOnUtc).IsRequired();
            b.Property(x => x.Status).HasConversion<int>().IsRequired();
            b.Property(x => x.TryCount).IsRequired();
            b.Property(x => x.LastError);
            b.Property(x => x.NextAttemptUtc);

            b.HasIndex(x => new { x.Status, x.NextAttemptUtc });
            b.HasIndex(x => x.OccurredOnUtc);
        }
    }
}
