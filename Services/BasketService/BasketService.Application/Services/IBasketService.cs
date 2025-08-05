using BasketService.Domain.Entities;

namespace BasketService.Application.Services;

public interface IBasketService
{
    Task<Basket?> GetBasketAsync(string userName);
    Task<Basket> UpdateBasketAsync(Basket basket);
    Task DeleteBasketAsync(string userName);
}