
using AutoMapper;
using FluentValidation;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Services
{
    public class ProductClassServiceTests
    {
        private readonly ProductClassService _productClassService;
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IValidator<ProductDto>> _validatorMock = new();
        private readonly Mock<ICurrentUserService> _userMock = new();
        private readonly Mock<IAuditLogService> _auditLogServiceMock = new();

        public ProductClassServiceTests()
        {
            _productClassService = new ProductClassService(
                _productRepositoryMock.Object,
                _mapperMock.Object,
                _validatorMock.Object,
                _userMock.Object,
                _auditLogServiceMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_ValidProduct_ShouldCallRepository()
        {
            var productDto = new ProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 123,
                Category = "Test",
                Stock = 10
            };

            var product = new Product();
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(product);
            _userMock.Setup(u => u.UserName).Returns("TestUser");

            await _productClassService.AddAsync(productDto);

            _productRepositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p =>
                            p.CreatedBy == "TestUser"
                        )), Times.Once);
        }

        [Fact]
        public async Task AddAsync_InvalidProduct_ShouldThrowValidationException()
        {
            var productDto = new ProductDto { Name = "", Price = -1 };

            var failures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name cannot be empty"),
                new FluentValidation.Results.ValidationFailure("Price", "Price must be greater than or equal to 0")
            };

            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));

            await Assert.ThrowsAsync<ValidationException>(() => _productClassService.AddAsync(productDto));
        }

        [Fact]
        public async Task AddAsync_ValidProduct_ShouldCallAuditLogService()
        {
            var productDto = new ProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 123,
                Category = "Test",
                Stock = 10
            };

            var product = new Product();
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(product);
            _userMock.Setup(u => u.UserName).Returns("TestUser");

            await _productClassService.AddAsync(productDto);

            _auditLogServiceMock.Verify(a => a.AuditLog(
                It.Is<string>(action => action == "Create"),
                It.Is<string>(entity => entity == nameof(Product)),
                It.IsAny<string>(),
                It.Is<string>(user => user == "TestUser")
            ), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ValidProduct_ShouldCallRepositoryUpdate()
        {
            var productDto = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150,
                Category = "Updated Category",
                Stock = 20
            };

            var existingProduct = new Product { Id = productDto.Id };

            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productDto.Id))
                                  .ReturnsAsync(existingProduct);

            _mapperMock.Setup(m => m.Map(productDto, existingProduct));

            _userMock.Setup(u => u.UserName).Returns("TestUser");

            await _productClassService.UpdateAsync(productDto);

            _productRepositoryMock.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProductNotFound_ShouldThrowException()
        {
            var productDto = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "NonExisting Product",
                Description = "Does not exist",
                Price = 100,
                Category = "Category",
                Stock = 5
            };

            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productDto.Id))
                                  .ReturnsAsync((Product?)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _productClassService.UpdateAsync(productDto));

            Assert.Equal("Product not found!", exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_ValidId_ShouldCallRepository()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync(product);

            await _productClassService.DeleteAsync(productId);

            _productRepositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ProductNotFound_ShouldThrowException()
        {
            var productId = Guid.NewGuid();

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                                  .ReturnsAsync((Product?)null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _productClassService.DeleteAsync(productId));

            Assert.Equal("Product not found!", exception.Message);
        }

    }
}
