using MassTransit;
using ECommerce.Contracts.Orders;
using PaymentService.Application.Interfaces;

namespace PaymentService.API.Consumers
{
    public class PaymentOrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IOrderCreatedEventHandler _handler;
        public PaymentOrderCreatedConsumer(IOrderCreatedEventHandler handler) => _handler = handler;

        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
            => _handler.HandleAsync(context.Message, context.MessageId ?? Guid.Empty, context.CancellationToken);
    }
}
