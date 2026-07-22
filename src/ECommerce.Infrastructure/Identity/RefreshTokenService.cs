using System.Security.Cryptography;
using System.Text;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using ECommerce.UseCases.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Identity;

public sealed class RefreshTokenService(
    IdentityStoreDbContext dbContext,
    IOptions<JwtSettings> settings) : IRefreshTokenService
{
    private readonly JwtSettings _settings = settings.Value;

    public async Task<RefreshTokenIssueResult> IssueAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var (plainToken, entity) = CreateTokenEntity(userId);
        dbContext.RefreshTokens.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenIssueResult(userId, plainToken, entity.ExpiresAtUtc);
    }

    public async Task<Result<RefreshTokenIssueResult>> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (existing is null)
            return Result<RefreshTokenIssueResult>.Failure(IdentityErrors.InvalidRefreshToken);

        if (existing.IsRevoked)
        {
            await RevokeAllActiveForUserAsync(existing.UserId, cancellationToken);
            return Result<RefreshTokenIssueResult>.Failure(IdentityErrors.InvalidRefreshToken);
        }

        if (existing.IsExpired)
            return Result<RefreshTokenIssueResult>.Failure(IdentityErrors.RefreshTokenExpired);

        var (plainToken, replacement) = CreateTokenEntity(existing.UserId);
        existing.Revoke(replacement.TokenHash);
        dbContext.RefreshTokens.Add(replacement);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<RefreshTokenIssueResult>.Success(
            new RefreshTokenIssueResult(existing.UserId, plainToken, replacement.ExpiresAtUtc));
    }

    public async Task<Result> RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (existing is null)
            return Result.Failure(IdentityErrors.InvalidRefreshToken);

        if (!existing.IsRevoked)
        {
            existing.Revoke();
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private (string PlainToken, RefreshToken Entity) CreateTokenEntity(Guid userId)
    {
        var plainToken = GenerateSecureToken();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
        var entity = RefreshToken.Create(
            Guid.NewGuid(),
            userId,
            HashToken(plainToken),
            expiresAt);

        return (plainToken, entity);
    }

    private async Task RevokeAllActiveForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activeTokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke();

        if (activeTokens.Count > 0)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateSecureToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
