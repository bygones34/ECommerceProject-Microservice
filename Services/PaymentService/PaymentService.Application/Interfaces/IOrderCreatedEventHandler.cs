using ECommerce.Contracts.Orders;

namespace PaymentService.Application.Interfaces
{
    public interface IOrderCreatedEventHandler
    {
        Task HandleAsync(OrderCreatedEvent evt, Guid messageId, CancellationToken ct);
    }
}
