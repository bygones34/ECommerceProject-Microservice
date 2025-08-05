namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = null!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string AddressLine { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string District { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = new();

    private Order()
    {
        
    }
    
    public void AddOrderItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        OrderItems.Add(item);
    }

    
    public static Order Create(string userName, string addressLine, string city, string district, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("BuyerId boş olamaz", nameof(userName));

        if (string.IsNullOrWhiteSpace(addressLine))
            throw new ArgumentException("Adres boş olamaz", nameof(addressLine));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("Şehir boş olamaz", nameof(city));

        if (string.IsNullOrWhiteSpace(district))
            throw new ArgumentException("İlçe boş olamaz", nameof(district));

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("Posta kodu boş olamaz", nameof(zipCode));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Ülke boş olamaz", nameof(country));

        return new Order
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            AddressLine = addressLine,
            City = city,
            District = district,
            ZipCode = zipCode,
            Country = country,
            OrderDate = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };
    }
}