namespace OrderService.Application.Features.DTOs;

public class OrderItemDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}