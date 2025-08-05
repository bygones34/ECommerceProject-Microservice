using FluentValidation;
using ProductService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .MaximumLength(100).WithMessage("Ürün adı en fazla 100 karakter olabilir.");

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
                .NotNull().WithMessage("Açıklama boş olamaz.")
                .NotEmpty().WithMessage("Açıklama boş olamaz.");

            RuleFor(p => p.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat sıfırdan küçük olamaz.");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stok sıfırdan küçük olamaz.");

            RuleFor(x => x.Category)
                .NotNull().WithMessage("Kategori boş olamaz.")
                .NotEmpty().WithMessage("Kategori boş olamaz.");
        }
    }
}