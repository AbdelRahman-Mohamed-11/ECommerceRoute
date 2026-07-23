using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithErrorCode("Identity.Email.Invalid")
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithErrorCode("Identity.Password.Invalid")
            .WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName))
            .WithErrorCode("Identity.DisplayName.TooLong")
            .WithMessage("Display name cannot exceed 100 characters.");
    }
}
