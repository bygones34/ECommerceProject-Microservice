using MediatR;
using OrderService.Application.Features.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersByBuyerIdAsync(request.UserName);

        return orders.Select(order => new OrderDto
        {
            OrderId = order.Id,
            UserName = order.UserName,
            AddressLine = order.AddressLine,
            City = order.City,
            District = order.District,
            ZipCode = order.ZipCode,
            Country = order.Country,
            OrderDate = order.OrderDate,
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        }).ToList();
    }

}
