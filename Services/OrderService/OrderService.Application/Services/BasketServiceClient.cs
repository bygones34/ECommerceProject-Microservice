using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrderService.Application.Models;

namespace OrderService.Application.Services;

public class BasketServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BasketServiceClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BasketServiceClient(HttpClient httpClient, ILogger<BasketServiceClient> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BasketDto?> GetBasketAsync(string userName)
    {
        try
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetFromJsonAsync<BasketDto>($"/basket/{userName}");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured fetching basket data!");
            return null;
        }
    }

    public async Task DeleteBasketAsync(string userName)
    {
        var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        
        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
        }
        
        var response = await _httpClient.DeleteAsync($"/basket/{userName}");
        response.EnsureSuccessStatusCode();
    }
}