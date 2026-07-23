using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithErrorCode("Identity.Email.Invalid")
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(4, 10)
            .WithErrorCode("Identity.Code.Invalid")
            .WithMessage("Verification code is required.");
    }
}
