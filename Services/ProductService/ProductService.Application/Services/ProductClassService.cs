using AutoMapper;
using FluentValidation;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using System.Runtime.InteropServices;

namespace ProductService.Application.Services
{
    public class ProductClassService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductDto> _validator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogService _auditLogService;

        public ProductClassService(IProductRepository productRepository, IMapper mapper, IValidator<ProductDto> validator, ICurrentUserService currentUserService, IAuditLogService auditLogService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _validator = validator;
            _currentUserService = currentUserService;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task AddAsync(ProductDto productDto)
        {
            var validationResult = await _validator.ValidateAsync(productDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var product = _mapper.Map<Product>(productDto);
            product.CreatedBy = _currentUserService.UserName ?? "Unknown";
            product.CreatedAt = DateTime.UtcNow;

            await _productRepository.AddAsync(product);
            productDto.Id = product.Id;

            await _auditLogService.AuditLog("Create", "Product", product.Id.ToString(), product.CreatedBy ?? "Unknown");

        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            var validationResult = await _validator.ValidateAsync(productDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingProduct = await _productRepository.GetByIdAsync(productDto.Id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found!");
            }

            _mapper.Map(productDto, existingProduct);

            existingProduct.UpdatedBy = _currentUserService.UserName ?? "Unknown";
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(existingProduct);

            await _auditLogService.AuditLog("Update", "Product", existingProduct.Id.ToString(), existingProduct.UpdatedBy ?? "Unknown");
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found!");
            }

            await _productRepository.DeleteAsync(id);

            await _auditLogService.AuditLog("Delete", "Product", id.ToString(), _currentUserService.UserName ?? "Unknown");
        }
    }
}
