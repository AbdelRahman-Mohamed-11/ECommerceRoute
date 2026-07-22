using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUserService currentUser,
    IIdentityService identityService)
    : IRequestHandler<GetCurrentUserQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<UserProfileResponse>.Failure(IdentityErrors.InvalidCredentials);

        var userResult = await identityService.GetUserByIdAsync(
            currentUser.UserId.Value,
            cancellationToken);

        if (userResult.IsFailure)
            return Result<UserProfileResponse>.Failure(userResult.Error);

        var user = userResult.Value;
        return Result<UserProfileResponse>.Success(
            new UserProfileResponse(user.UserId, user.Email, user.DisplayName));
    }
}
