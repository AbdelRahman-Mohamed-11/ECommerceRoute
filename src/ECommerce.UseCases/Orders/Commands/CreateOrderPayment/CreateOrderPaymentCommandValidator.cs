using FluentValidation;

namespace ECommerce.UseCases.Orders.Commands.CreateOrderPayment;

public sealed class CreateOrderPaymentCommandValidator : AbstractValidator<CreateOrderPaymentCommand>
{
    public CreateOrderPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithErrorCode("Order.InvalidId")
            .WithMessage("Order id is required.");
    }
}
