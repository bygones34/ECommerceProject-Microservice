using System.Security.Claims;
using AutoMapper;
using BasketService.API.DTOs;
using BasketService.Application.Services;
using BasketService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BasketService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(IBasketService basketService, IMapper mapper)
    {
        _basketService = basketService;
        _mapper = mapper;
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<Basket>> GetBasketAsync(string userName)
    {
        var basket = await _basketService.GetBasketAsync(userName);
        if (basket == null)
            return NotFound("Basket is empty.");

        return Ok(_mapper.Map<BasketDto>(basket));
    }

    [HttpPost]
    public async Task<ActionResult<BasketDto>> UpdateBasketAsync([FromBody] BasketDto basketDto)
    {
        var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"UserId (nameidentifier): {userName}");

        if (string.IsNullOrEmpty(userName))
        {
            return Unauthorized("User not authenticated.");
        }
        
        basketDto.UserName = userName;

        var updatedBasket = await _basketService.UpdateBasketAsync(_mapper.Map<Basket>(basketDto));

        return Ok(_mapper.Map<BasketDto>(updatedBasket));
    }
    
    [HttpGet("test-user")]
    public IActionResult GetTestUser()
    {
        return Ok(new
        {
            User.Identity?.IsAuthenticated,
            User.Identity?.Name,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [HttpDelete("{userName}")]
    public async Task<ActionResult> DeleteBasketAsync(string userName)
    {
        var basket =  await _basketService.GetBasketAsync(userName);
        if (basket == null)
            return NotFound("Basket is either empty or does not exist!");
        
        await _basketService.DeleteBasketAsync(userName);
        return Ok("Basket deleted successfully!");
    }
}