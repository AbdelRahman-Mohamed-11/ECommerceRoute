using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler
    : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailVerificationCodeStore _verificationCodeStore;

    public ConfirmEmailCommandHandler(
        IIdentityService identityService,
        IEmailVerificationCodeStore verificationCodeStore)
    {
        _identityService = identityService;
        _verificationCodeStore = verificationCodeStore;
    }

    public async Task<Result> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var code = request.Code.Trim();

        var userResult = await _identityService.GetUserByEmailAsync(email, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure(IdentityErrors.InvalidVerificationCode);

        if (await _identityService.IsEmailConfirmedAsync(email, cancellationToken))
            return Result.Failure(IdentityErrors.EmailAlreadyConfirmed);

        var isValid = await _verificationCodeStore.ValidateAndConsumeAsync(
            email,
            code,
            cancellationToken);

        if (!isValid)
            return Result.Failure(IdentityErrors.InvalidVerificationCode);

        return await _identityService.ConfirmEmailAsync(email, cancellationToken);
    }
}
