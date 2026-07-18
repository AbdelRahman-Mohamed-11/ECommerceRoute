using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler
    : IRequestHandler<ConfirmEmailCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailVerificationCodeStore _verificationCodeStore;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public ConfirmEmailCommandHandler(
        IIdentityService identityService,
        IEmailVerificationCodeStore verificationCodeStore,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _verificationCodeStore = verificationCodeStore;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var code = request.Code.Trim();

        var userResult = await _identityService.GetUserByEmailAsync(email, cancellationToken);
        if (userResult.IsFailure)
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidVerificationCode);

        if (await _identityService.IsEmailConfirmedAsync(email, cancellationToken))
            return Result<AuthResponse>.Failure(IdentityErrors.EmailAlreadyConfirmed);

        var isValid = await _verificationCodeStore.ValidateAndConsumeAsync(
            email,
            code,
            cancellationToken);

        if (!isValid)
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidVerificationCode);

        var confirmResult = await _identityService.ConfirmEmailAsync(email, cancellationToken);
        if (confirmResult.IsFailure)
            return Result<AuthResponse>.Failure(confirmResult.Error);

        var user = userResult.Value;
        var roles = await _identityService.GetRolesAsync(user.UserId, cancellationToken);
        var accessToken = _jwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Email,
            user.DisplayName,
            roles);
        var refreshToken = await _refreshTokenService.IssueAsync(user.UserId, cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.UserId,
            user.Email,
            user.DisplayName,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc));
    }
}
