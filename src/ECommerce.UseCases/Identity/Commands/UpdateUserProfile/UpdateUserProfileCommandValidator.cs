using FluentValidation;

namespace ECommerce.UseCases.Identity.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName))
            .WithErrorCode("Identity.DisplayName.TooLong")
            .WithMessage("Display name cannot exceed 100 characters.");
    }
}
