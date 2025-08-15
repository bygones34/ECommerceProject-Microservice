
using OrderService.Domain.Outbox;

namespace OrderService.Application.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessage msg, CancellationToken ct = default);
        Task<List<OutboxMessage>> GetPendingBatchAsync(int take, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
