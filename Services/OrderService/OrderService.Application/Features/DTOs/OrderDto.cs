namespace OrderService.Application.Features.DTOs;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public int Status { get; set; }

    public List<OrderItemDto> OrderItems { get; set; } = new();
}