using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Models;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<RefreshTokenIssueResult>> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}
