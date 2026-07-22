using System.Security.Cryptography;
using System.Text;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Stores a SHA-256 hash of the verification code in HybridCache (one-time use).
/// </summary>
public sealed class EmailVerificationCodeStore(
    HybridCache cache,
    IOptions<EmailVerificationSettings> settings) : IEmailVerificationCodeStore
{
    private const string KeyPrefix = "email-verification:";

    public async Task SaveAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default)
    {
        var options = settings.Value;
        var key = BuildKey(email);
        var entry = new VerificationEntry(
            CodeHash: HashCode(code),
            FailedAttempts: 0);

        await cache.SetAsync(
            key,
            entry,
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(options.ExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(
                    Math.Min(5, options.ExpirationMinutes))
            },
            cancellationToken: cancellationToken);
    }

    public async Task<bool> ValidateAndConsumeAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default)
    {
        var options = settings.Value;
        var key = BuildKey(email);
        var entry = await TryGetEntryAsync(key, cancellationToken);

        if (entry is null)
            return false;

        if (entry.FailedAttempts >= options.MaxFailedAttempts)
        {
            await cache.RemoveAsync(key, cancellationToken);
            return false;
        }

        var expected = Encoding.UTF8.GetBytes(entry.CodeHash);
        var actual = Encoding.UTF8.GetBytes(HashCode(code));

        if (expected.Length != actual.Length
            || !CryptographicOperations.FixedTimeEquals(expected, actual))
        {
            entry = entry with { FailedAttempts = entry.FailedAttempts + 1 };

            if (entry.FailedAttempts >= options.MaxFailedAttempts)
            {
                await cache.RemoveAsync(key, cancellationToken);
                return false;
            }

            await cache.SetAsync(
                key,
                entry,
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(options.ExpirationMinutes),
                    LocalCacheExpiration = TimeSpan.FromMinutes(
                        Math.Min(5, options.ExpirationMinutes))
                },
                cancellationToken: cancellationToken);

            return false;
        }

        await cache.RemoveAsync(key, cancellationToken);
        return true;
    }

    private async Task<VerificationEntry?> TryGetEntryAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var found = true;

        var value = await cache.GetOrCreateAsync(
            key,
            _ =>
            {
                found = false;
                return ValueTask.FromResult<VerificationEntry?>(null);
            },
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCacheWrite
                    | HybridCacheEntryFlags.DisableDistributedCacheWrite
            },
            cancellationToken: cancellationToken);

        return found ? value : null;
    }

    private static string BuildKey(string email) =>
        KeyPrefix + email.Trim().ToLowerInvariant();

    private static string HashCode(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code.Trim()));
        return Convert.ToHexString(bytes);
    }

    private sealed record VerificationEntry(string CodeHash, int FailedAttempts);
}
