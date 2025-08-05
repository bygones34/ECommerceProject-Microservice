using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Events;
using System.Threading.Tasks;

namespace OrderService.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("OrderCreatedEvent alındı. OrderId={OrderId}, UserName={UserName}, TotalPrice={TotalPrice}, ItemCount={Count}",
            evt.OrderId, evt.UserName, evt.TotalPrice, evt.Items.Count);
        
        return Task.CompletedTask;
    }
}