
using MediatR;
using OrderService.Application.Features.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, ct);
            if (order == null)
            {
                return null;
            }

            return new OrderDto
            {
                OrderId = order.Id,
                UserName = order.UserName,
                AddressLine = order.AddressLine,
                City = order.City,
                District = order.District,
                ZipCode = order.ZipCode,
                Country = order.Country,
                OrderDate = order.OrderDate,
                Status = (int)order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}
