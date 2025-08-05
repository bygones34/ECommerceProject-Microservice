using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Events;
using OrderService.Application.Interfaces;
using OrderService.Application.Services;
using OrderService.Domain.Entities;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly BasketServiceClient _basketClient;
    private readonly ProductServiceClient _productServiceClient;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        BasketServiceClient basketClient,
        ProductServiceClient productServiceClient,
        ILogger<CreateOrderCommandHandler> logger,
        IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _basketClient = basketClient;
        _productServiceClient = productServiceClient;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketClient.GetBasketAsync(request.UserName);
        if (basket == null || !basket.BasketItems.Any())
            throw new InvalidOperationException("Couldn't fetch basket, it's either unavailable or empty!");

        var orderItems = new List<OrderItem>();

        foreach (var item in basket.BasketItems)
        {
            decimal finalPrice = item.Price;

            try
            {
                var product = await _productServiceClient.GetProductAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product wasn't able to be fetched from ProductService." +
                                       " ProductId: {ProductId}," +
                                       " Basket price is accepted: {Price}", item.ProductId, item.Price);
                }
                else
                {
                    if (product.Price != item.Price)
                    {
                        _logger.LogInformation(
                            "Price was updated: ProductId={ProductId}, BasketPrice={BasketPrice}," +
                            " CurrentPrice={CurrentPrice}",
                            item.ProductId, item.Price, product.Price);

                        finalPrice = product.Price;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while accessing ProductService." +
                                       " Basket price will be accepted. ProductId: {ProductId}", item.ProductId);
            }

            var orderItem = OrderItem.Create(
                item.ProductId,
                item.ProductName,
                finalPrice,
                item.Quantity);

            orderItems.Add(orderItem);
        }

        var order = Order.Create(
            request.UserName,
            request.AddressLine,
            request.City,
            request.District,
            request.ZipCode,
            request.Country
        );

        foreach (var item in orderItems)
        {
            order.AddOrderItem(item);
        }

        await _orderRepository.AddOrderAsync(order);
        
        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.UserName,
            order.OrderItems.Select(i => new OrderItemEventDto(
                i.ProductId,
                i.ProductName,
                i.Price,
                i.Quantity)).ToList(),
            order.OrderItems.Sum(i => i.Price * i.Quantity),
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(orderCreatedEvent);

        await _basketClient.DeleteBasketAsync(request.UserName);

        return order.Id;
    }
}
