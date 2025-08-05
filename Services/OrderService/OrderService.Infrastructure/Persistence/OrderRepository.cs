using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _orderDbContext;

    public OrderRepository(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task<List<Order>> GetOrdersByBuyerIdAsync(string userName)
    {
        return await _orderDbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserName == userName)
            .ToListAsync();
    }
        
    public async Task AddOrderAsync(Order order)
    {
        await _orderDbContext.AddAsync(order);
        await _orderDbContext.SaveChangesAsync();
    }
}