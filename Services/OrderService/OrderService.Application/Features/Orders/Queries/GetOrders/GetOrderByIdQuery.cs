using MediatR;
using OrderService.Application.Features.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrders
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;

}
