namespace BasketService.Domain.Entities;

public class Basket
{
    public string UserName { get; set; }
    public List<BasketItem> BasketItems { get; set; }
    
    public decimal TotalPrice => BasketItems.Sum(b => b.Price * b.Quantity);
}