using BasketService.API.DTOs;
using FluentValidation;

namespace BasketService.API.Validators;

public class BasketItemDtoValidator : AbstractValidator<BasketItemDto>
{
    public BasketItemDtoValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty().WithMessage("ProductId boş olamaz.");

        RuleFor(i => i.ProductName)
            .NotEmpty().WithMessage("ProductName boş olamaz.");

        RuleFor(i => i.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Adet 0'dan büyük olmalıdır.");
    }
}