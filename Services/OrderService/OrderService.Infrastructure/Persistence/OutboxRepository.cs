using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Outbox;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.Persistence
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly OrderDbContext _context;
        public OutboxRepository(OrderDbContext context) => _context = context;

        public async Task AddAsync(OutboxMessage msg, CancellationToken ct = default)
        => await _context.OutboxMessages.AddAsync(msg, ct);

        public Task<List<OutboxMessage>> GetPendingBatchAsync(int take, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return _context.OutboxMessages
                .Where(x => x.Status == OutboxStatus.Pending && (x.NextAttemptUtc == null || x.NextAttemptUtc <= now))
                .OrderBy(x => x.OccurredOnUtc)
                .Take(take)
                .ToListAsync(ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
    }
}
