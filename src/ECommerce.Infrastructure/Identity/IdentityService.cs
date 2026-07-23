using ECommerce.Domain.Constants;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result<AuthUserSnapshot>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e =>
                    e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return Result<AuthUserSnapshot>.Failure(IdentityErrors.EmailAlreadyExists);
            }

            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.CreateFailed(message));
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }

    public async Task<Result<AuthUserSnapshot>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.InvalidCredentials);

        var isValid = await userManager.CheckPasswordAsync(user, password);
        if (!isValid)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.EmailNotConfirmed);

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }

    public async Task<Result<AuthUserSnapshot>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }

    public async Task<Result<AuthUserSnapshot>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }

    public async Task<Result<AuthUserSnapshot>> UpdateProfileAsync(
        Guid userId,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        user.DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.CreateFailed(message));
        }

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }

    public async Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(IdentityErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Failure(IdentityErrors.EmailAlreadyConfirmed);

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(IdentityErrors.CreateFailed(
                string.Join(" ", result.Errors.Select(e => e.Description))));
    }

    public async Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user?.EmailConfirmed ?? false;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
