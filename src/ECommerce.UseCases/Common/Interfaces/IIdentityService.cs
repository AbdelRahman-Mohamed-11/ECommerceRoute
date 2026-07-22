using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Models;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<IdentityUserInfo>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserInfo>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken cancellationToken = default);

    Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserInfo>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserInfo>> UpdateProfileAsync(
        Guid userId,
        string? displayName,
        CancellationToken cancellationToken = default);
}
