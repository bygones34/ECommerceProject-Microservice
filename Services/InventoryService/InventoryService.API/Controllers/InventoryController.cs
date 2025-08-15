using InventoryService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryController(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetStock(string productId)
    {
        var item = await _inventoryRepository.GetByProductIdAsync(productId);
        if (item == null) return NotFound();

        return Ok(new
        {
            item.ProductId,
            item.AvailableQuantity
        });
    }

    [HttpPost("{productId}/increase")]
    public async Task<IActionResult> Increase(string productId, [FromQuery] int amount)
    {
        var item = await _inventoryRepository.GetByProductIdAsync(productId);
        if (item == null)
        {
            item = new Domain.Entities.InventoryItem(productId, amount);
            await _inventoryRepository.AddAsync(item);
        }
        else
        {
            item.Increase(amount);
            await _inventoryRepository.UpdateAsync(item);
        }

        await _inventoryRepository.SaveChangesAsync();
        return Ok();
    }
}