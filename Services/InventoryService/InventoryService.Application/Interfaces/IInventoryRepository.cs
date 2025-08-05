using InventoryService.Domain.Entities;

namespace InventoryService.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(string productId);
    Task AddAsync(InventoryItem item);
    Task UpdateAsync(InventoryItem item);
    Task<bool> SaveChangesAsync();
}