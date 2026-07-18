using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.AddUserAddress;

public sealed class AddUserAddressCommandValidator : AbstractValidator<AddUserAddressCommand>
{
    public AddUserAddressCommandValidator()
    {
        RuleFor(x => x.Label).NotEmpty().MaximumLength(64);
        RuleFor(x => x.RecipientFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.RecipientLastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
    }
}
