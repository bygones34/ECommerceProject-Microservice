using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersByBuyerIdAsync(string userName);
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken ct = default);
    Task AddOrderAsync(Order order);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}