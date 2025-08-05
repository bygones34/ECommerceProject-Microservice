using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromBody] ProductDto productDto)
        {
            var username = User.Identity?.Name ?? "Unknown";
            Console.WriteLine($"[Log] Product added by: {username}");

            await _productService.AddAsync(productDto);
            return Ok("Product successfully added!");
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] Guid productId, [FromBody] ProductDto productDto)
        {
            if (productId != productDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body!");
            }
            var username = User.Identity?.Name ?? "Unknown";
            Console.WriteLine($"[Log] Product updated by: {username}");

            await _productService.UpdateAsync(productDto);
            return Ok("Product successfully updated!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(Guid id)
        {
            var username = User.Identity?.Name ?? "Unknown";
            Console.WriteLine($"[Log] Product deleted by: {username}");

            await _productService.DeleteAsync(id);
            return Ok("Product successfully deleted!");
        }

        [HttpGet("crash")]
        public IActionResult Crash()
        {
            throw new Exception("Bir şeyler ters gitti!");
        }
    }
}