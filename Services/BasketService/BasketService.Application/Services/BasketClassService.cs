using BasketService.Domain.Entities;
using BasketService.Infrastructure.Repositories;

namespace BasketService.Application.Services;

public class BasketClassService : IBasketService
{
    private readonly IBasketRepository _basketRepository;

    public BasketClassService(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public Task<Basket?> GetBasketAsync(string userName)
        => _basketRepository.GetBasketAsync(userName);

    public Task<Basket> UpdateBasketAsync(Basket basket)
        => _basketRepository.UpdateBasketAsync(basket);

    public Task DeleteBasketAsync(string userName)
        => _basketRepository.DeleteBasketAsync(userName);
}