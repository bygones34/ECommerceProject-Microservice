using ECommerce.Contracts.Payments;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Consumers
{
    public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(IOrderRepository orderRepository, ILogger<PaymentFailedConsumer> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var msg = context.Message;
            var order = await _orderRepository.GetOrderByIdAsync(msg.OrderId, context.CancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Order not found for PaymentFailed. OrderId = {OrderId}", msg.OrderId);
                return;
            }

            order.MarkFailed();
            await _orderRepository.UpdateAsync(order, context.CancellationToken);
            await _orderRepository.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Order marked as FAILED. OrderId = {OrderId}", msg.OrderId);
        }
    }
}
