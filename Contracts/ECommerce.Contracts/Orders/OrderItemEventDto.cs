namespace ECommerce.Contracts.Orders;

public sealed record OrderItemEventDto(
    string ProductId,
    string ProductName,
    decimal Price,
    int Quantity);
