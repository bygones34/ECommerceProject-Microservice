using BasketService.Domain.Entities;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace BasketService.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _database;

    public BasketRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<Basket?> GetBasketAsync(string userName)
    {
        var basketData = await _database.StringGetAsync(userName);
        return basketData.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<Basket>(basketData!);
    }

    public async Task<Basket> UpdateBasketAsync(Basket basket)
    {
        if (string.IsNullOrWhiteSpace(basket.UserName))
            throw new ArgumentException("Basket.UserName cannot be null or empty");

        var json =  JsonConvert.SerializeObject(basket);
        await _database.StringSetAsync(basket.UserName, json);
        return basket;
    }

    public async Task DeleteBasketAsync(string userName)
    {
        await _database.KeyDeleteAsync(userName);
    }
}