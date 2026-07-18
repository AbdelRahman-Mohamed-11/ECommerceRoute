using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _refreshTokenService = refreshTokenService;
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidRefreshToken);

        var rotateResult = await _refreshTokenService.RotateAsync(
            request.RefreshToken.Trim(),
            cancellationToken);

        if (rotateResult.IsFailure)
            return Result<AuthResponse>.Failure(rotateResult.Error);

        var refresh = rotateResult.Value;
        var userResult = await _identityService.GetUserByIdAsync(
            refresh.UserId,
            cancellationToken);

        if (userResult.IsFailure)
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidRefreshToken);

        var user = userResult.Value;
        var roles = await _identityService.GetRolesAsync(user.UserId, cancellationToken);
        var accessToken = _jwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Email,
            user.DisplayName,
            roles);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.UserId,
            user.Email,
            user.DisplayName,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refresh.Token,
            refresh.ExpiresAtUtc));
    }
}
