using ECommerce.Contracts.Payments;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Consumers
{
    public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentSucceededConsumer> _logger;

        public PaymentSucceededConsumer(IOrderRepository orderRepository, ILogger<PaymentSucceededConsumer> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
        {
            var msg = context.Message;
            var order = await _orderRepository.GetOrderByIdAsync(msg.OrderId, context.CancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Order not found for PaymentSucceeded. OrderId={OrderId}", msg.OrderId);
                return;
            }

            order.MarkPaid();
            await _orderRepository.UpdateAsync(order, context.CancellationToken);
            await _orderRepository.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Order marked as PAID. OrderId = {OrderId}", msg.OrderId);
        }
    }
}
