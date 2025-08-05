using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrderService.Application.Features.DTOs;

namespace OrderService.Application.Services;

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ProductDto?> GetProductAsync(string productId)
    {
        try
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(accessToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));

            var response = await _httpClient.GetFromJsonAsync<ProductDto>($"/products/{productId}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ürün bilgisi alınırken hata oluştu.");
            return null;
        }
    }

    
}