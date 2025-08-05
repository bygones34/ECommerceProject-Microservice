namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    private OrderItem() { }
    
    public static OrderItem Create(string productId, string productName, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId boş olamaz", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Ürün adı boş olamaz", nameof(productName));

        if (price <= 0)
            throw new ArgumentException("Fiyat 0'dan büyük olmalıdır", nameof(price));

        if (quantity <= 0)
            throw new ArgumentException("Adet 0'dan büyük olmalıdır", nameof(quantity));

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        };
    }
}