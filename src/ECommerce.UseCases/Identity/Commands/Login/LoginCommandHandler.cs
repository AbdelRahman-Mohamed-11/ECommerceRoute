using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var validateResult = await _identityService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (validateResult.IsFailure)
        {
            return Result<AuthResponse>.Failure(
                validateResult.Error.Code == IdentityErrors.EmailNotConfirmed.Code
                    ? IdentityErrors.EmailNotConfirmed
                    : IdentityErrors.InvalidCredentials);
        }

        var user = validateResult.Value;
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
