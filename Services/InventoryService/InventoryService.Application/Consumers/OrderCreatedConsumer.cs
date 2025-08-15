using MassTransit;
using Microsoft.Extensions.Logging;
using ECommerce.Contracts.Orders;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IInventoryRepository _inventoryRepo;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IInventoryRepository inventoryRepository, ILogger<OrderCreatedConsumer> logger)
    {
        _inventoryRepo = inventoryRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("OrderCreatedEvent received. OrderId={OrderId}, UserName={UserName}, TotalPrice={TotalPrice}",
            evt.OrderId, evt.UserName, evt.TotalPrice);

        foreach (var item in evt.Items)
        {
            var inventoryItem = await _inventoryRepo.GetByProductIdAsync(item.ProductId);
            if (inventoryItem == null)
            {
                _logger.LogWarning("No stock records, creating a new one! ProductId={ProductId}", item.ProductId);
                inventoryItem = new InventoryItem(item.ProductId, 0);
                await _inventoryRepo.AddAsync(inventoryItem);
            }

            var success = inventoryItem.TryDecrease(item.Quantity);
            if (!success)
            {
                _logger.LogWarning("Insufficient stock! ProductId={ProductId}, Requested={Requested}, Available={Available}",
                    item.ProductId, item.Quantity, inventoryItem.AvailableQuantity);
            }
            else
            {
                await _inventoryRepo.UpdateAsync(inventoryItem);
                _logger.LogInformation("Stock updated! ProductId={ProductId}, Remaining={Remaining}", item.ProductId, inventoryItem.AvailableQuantity);
            }
        }

        await _inventoryRepo.SaveChangesAsync();
    }
}