using BasketService.API.DTOs;
using FluentValidation;

namespace BasketService.API.Validators;

public class BasketDtoValidator : AbstractValidator<BasketDto>
{
    public BasketDtoValidator()
    {
        RuleFor(b => b.UserName)
            .NotEmpty().WithMessage("UserName boş olamaz.");

        RuleForEach(b => b.BasketItems).SetValidator(new BasketItemDtoValidator());
    }
}