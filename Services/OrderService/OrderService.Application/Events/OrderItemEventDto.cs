namespace OrderService.Application.Events;

public sealed record OrderItemEventDto(
    string ProductId,
    string ProductName,
    decimal Price,
    int Quantity);