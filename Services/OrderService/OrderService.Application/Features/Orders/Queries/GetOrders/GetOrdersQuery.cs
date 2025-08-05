using MediatR;
using OrderService.Application.Features.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQuery : IRequest<List<OrderDto>>
{
    public string UserName { get; }

    public GetOrdersQuery(string userName)
    {
        UserName = userName;
    }
}