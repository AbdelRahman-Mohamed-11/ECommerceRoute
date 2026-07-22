using ECommerce.Domain.Constants;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result<IdentityUserInfo>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
            return Result<IdentityUserInfo>.Failure(IdentityErrors.UserNotFound);

        return Result<IdentityUserInfo>.Success(ToInfo(user));
    }

    public async Task<Result<IdentityUserInfo>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
            return Result<IdentityUserInfo>.Failure(IdentityErrors.UserNotFound);

        return Result<IdentityUserInfo>.Success(ToInfo(user));
    }

    public async Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return user?.EmailConfirmed == true;
    }

    public async Task<Result<Guid>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = false,
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim()
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var message = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<Guid>.Failure(
                Error.Validation(IdentityErrors.CreateUserFailed.Code, message));
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return Result.Failure(IdentityErrors.UserNotFound);

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return Result.Failure(IdentityErrors.CreateUserFailed);

        return Result.Success();
    }

    public async Task<Result<IdentityUserInfo>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
            return Result<IdentityUserInfo>.Failure(IdentityErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result<IdentityUserInfo>.Failure(IdentityErrors.EmailNotConfirmed);

        if (!await userManager.CheckPasswordAsync(user, password))
            return Result<IdentityUserInfo>.Failure(IdentityErrors.InvalidCredentials);

        return Result<IdentityUserInfo>.Success(ToInfo(user));
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<Result<IdentityUserInfo>> UpdateProfileAsync(
        Guid userId,
        string? displayName,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.Email))
            return Result<IdentityUserInfo>.Failure(IdentityErrors.UserNotFound);

        user.DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<IdentityUserInfo>.Failure(IdentityErrors.UpdateProfileFailed);

        return Result<IdentityUserInfo>.Success(ToInfo(user));
    }

    private static IdentityUserInfo ToInfo(ApplicationUser user) =>
        new(user.Id, user.Email!, user.DisplayName);
}
