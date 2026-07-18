using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithErrorCode("Identity.Email.Invalid")
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode("Identity.Password.Required")
            .WithMessage("Password is required.");
    }
}
