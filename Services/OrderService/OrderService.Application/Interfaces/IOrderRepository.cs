using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersByBuyerIdAsync(string userName);
    Task AddOrderAsync(Order order);
}