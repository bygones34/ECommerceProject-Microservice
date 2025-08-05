using FluentValidation;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.AddressLine).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();

        RuleFor(x => x.OrderItems).NotEmpty().WithMessage("There has to be at least one product in an order!");

        RuleForEach(x => x.OrderItems).ChildRules(items =>
        {
            items.RuleFor(x => x.ProductId).NotEmpty();
            items.RuleFor(x => x.ProductName).NotEmpty();
            items.RuleFor(x => x.Price).GreaterThan(0);
            items.RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}
