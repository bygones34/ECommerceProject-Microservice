using System.Text.Json.Serialization;

namespace OrderService.Application.Models;

public class BasketDto
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
    
    public List<BasketItemDto> BasketItems { get; set; }
    public decimal TotalPrice { get; init; }
}