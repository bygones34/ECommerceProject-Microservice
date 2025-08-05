using BasketService.Domain.Entities;

namespace BasketService.Infrastructure.Repositories;

public interface IBasketRepository
{
    Task<Basket?> GetBasketAsync(string userName);
    Task<Basket> UpdateBasketAsync(Basket basket);
    Task DeleteBasketAsync(string userName);
}