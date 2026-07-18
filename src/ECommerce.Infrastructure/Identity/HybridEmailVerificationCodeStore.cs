using System.Security.Cryptography;
using System.Text;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Teaching mode: stores the <strong>plain</strong> verification code in HybridCache
/// so students can inspect Redis / local cache and copy the code into confirm-email.
/// Production should store a hash instead of the plain code.
/// </summary>
public sealed class HybridEmailVerificationCodeStore(
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
            Code: code.Trim(),
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

        var expected = Encoding.UTF8.GetBytes(entry.Code);
        var actual = Encoding.UTF8.GetBytes(code.Trim());

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

    private sealed record VerificationEntry(string Code, int FailedAttempts);
}
