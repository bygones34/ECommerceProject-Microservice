using System.Text.Json.Serialization;

namespace BasketService.API.DTOs;

public class BasketItemDto
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; }
    
    [JsonPropertyName("productName")]
    public string ProductName { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}