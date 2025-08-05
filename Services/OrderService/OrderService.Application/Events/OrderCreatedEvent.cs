namespace OrderService.Application.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    string UserName,
    List<OrderItemEventDto> Items,
    decimal TotalPrice,
    DateTime CreatedAt);