using FluentValidation;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Tests.Services
{
    public class ProductClassServiceTests_InvalidInputs
    {
        private readonly IValidator<ProductDto> _validator;
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductClassService _service;

        public ProductClassServiceTests_InvalidInputs()
        {
            _validator = new ProductDtoValidator();
            _mockRepo = new Mock<IProductRepository>();
            _service = new ProductClassService(_mockRepo.Object, null, _validator, null, null);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowValidationException_WhenPriceIsNegative()
        {
            var invalidProduct = new ProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = -100,
                Category = "Elektronik",
                Stock = 10
            };

            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
            _service.AddAsync(invalidProduct));

            Assert.Contains(ex.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public async Task AddAsync_ShouldThrowValidationException_WhenNameIsEmpty()
        {
            var invalidProduct = new ProductDto
            {
                Name = "",
                Description = "Test Description",
                Price = 100,
                Category = "Elektronik",
                Stock = 25
            };

            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _service.AddAsync(invalidProduct));

            Assert.Contains(ex.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task AddAsync_ShouldThrowValidationException_WhenStockIsNegative()
        {
            // Arrange
            var invalidProduct = new ProductDto
            {
                Name = "Test Ürün",
                Description = "Test Açıklama",
                Price = 100,
                Category = "Elektronik",
                Stock = -5 // Negatif stok
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _service.AddAsync(invalidProduct));

            Assert.Contains(ex.Errors, e => e.PropertyName == "Stock");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task AddAsync_ShouldThrowValidationException_WhenCategoryIsNullOrEmpty(string? invalidCategory)
        {
            // Arrange
            var invalidProduct = new ProductDto
            {
                Name = "Test Ürün",
                Description = "Test Açıklama",
                Price = 199.99m,
                Category = invalidCategory,
                Stock = 10
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _service.AddAsync(invalidProduct));

            Assert.Contains(ex.Errors, e => e.PropertyName == "Category");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task AddAsync_ShouldThrowValidationException_WhenDescriptionIsNullOrEmpty(string? invalidDescription)
        {
            // Arrange
            var invalidProduct = new ProductDto
            {
                Name = "Test Ürün",
                Description = invalidDescription,
                Price = 199.99m,
                Category = "Test Kategori",
                Stock = 10
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _service.AddAsync(invalidProduct));

            Assert.Contains(ex.Errors, e => e.PropertyName == "Description");
        }
    }
}
