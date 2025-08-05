using MediatR;
using OrderService.Application.Features.DTOs;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public string UserName { get; set; } = null!;
    public string AddressLine {get; set;} = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string District { get; set; } = null!;

    public List<OrderItemDto> OrderItems { get; set; } = new();
}

