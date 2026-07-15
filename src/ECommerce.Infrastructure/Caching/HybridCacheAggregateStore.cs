using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Caching;

public sealed class HybridCacheAggregateStore<T>(
    HybridCache cache,
    IOptionsMonitor<CacheEntryPolicy> optionsMonitor) : ICachedAggregateStore<T>
    where T : class
{
    private readonly CacheEntryPolicy _options = optionsMonitor.Get(typeof(T).Name);

    public async Task<T?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var envelope = await cache.TryGetAsync<CacheEnvelope<T>>(key, cancellationToken);
        return envelope?.Payload;
    }

    public async Task<T> GetOrCreateAsync(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        var envelope = await cache.GetOrCreateAsync(
            key,
            async cancel =>
            {
                var value = await factory(cancel);
                var utcNow = DateTime.UtcNow;

                return new CacheEnvelope<T>
                {
                    Payload = value,
                    CreatedAtUtc = utcNow,
                    LastAccessedUtc = utcNow
                };
            },
            CreateEntryOptionsForNewEnvelope(),
            cancellationToken: cancellationToken);

        await RefreshExpirationIfNeededAsync(key, envelope, cancellationToken);

        return envelope.Payload;
    }

    public async Task SetAsync(string key, T value, CancellationToken cancellationToken = default)
    {
        var existing = await cache.TryGetAsync<CacheEnvelope<T>>(key, cancellationToken);
        var utcNow = DateTime.UtcNow;

        var envelope = new CacheEnvelope<T>
        {
            Payload = value,
            CreatedAtUtc = existing?.CreatedAtUtc ?? utcNow,
            LastAccessedUtc = utcNow
        };

        await SetOrRemoveIfExpiredAsync(key, envelope, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) =>
        await cache.RemoveAsync(key, ct);

    private async Task RefreshExpirationIfNeededAsync(
        string key,
        CacheEnvelope<T> envelope,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var age = utcNow - envelope.LastAccessedUtc;

        if (age < TimeSpan.FromMinutes(_options.SlidingRefreshThresholdMinutes))
            return;

        var refreshed = new CacheEnvelope<T>
        {
            Payload = envelope.Payload,
            CreatedAtUtc = envelope.CreatedAtUtc,
            LastAccessedUtc = utcNow
        };

        await SetOrRemoveIfExpiredAsync(key, refreshed, cancellationToken);
    }

    private async Task SetOrRemoveIfExpiredAsync(
        string key,
        CacheEnvelope<T> envelope,
        CancellationToken cancellationToken)
    {
        var expiration = CalculateExpiration(
            envelope.CreatedAtUtc,
            envelope.LastAccessedUtc,
            DateTime.UtcNow);

        if (expiration is null)
        {
            await cache.RemoveAsync(key, cancellationToken);
            return;
        }

        await cache.SetAsync(key, envelope, CreateEntryOptions(expiration.Value), cancellationToken: cancellationToken);
    }

    private HybridCacheEntryOptions CreateEntryOptionsForNewEnvelope()
    {
        var utcNow = DateTime.UtcNow;
        var expiration = CalculateExpiration(utcNow, utcNow, utcNow)
            ?? throw new InvalidOperationException("A newly created cache envelope must have a positive expiration.");

        return CreateEntryOptions(expiration);
    }

    private HybridCacheEntryOptions CreateEntryOptions(TimeSpan expiration)
    {
        var localExpiration = TimeSpan.FromMinutes(_options.LocalCacheExpirationMinutes);

        if (localExpiration > expiration)
            localExpiration = expiration;

        return new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = localExpiration
        };
    }

    private TimeSpan? CalculateExpiration(
        DateTime createdAtUtc,
        DateTime lastAccessedUtc,
        DateTime utcNow)
    {
        var absoluteRemaining = createdAtUtc
            .AddDays(_options.AbsoluteExpirationDays)
            .Subtract(utcNow);

        var slidingRemaining = lastAccessedUtc
            .AddDays(_options.SlidingExpirationDays)
            .Subtract(utcNow);

        if (absoluteRemaining <= TimeSpan.Zero || slidingRemaining <= TimeSpan.Zero)
            return null;

        return absoluteRemaining <= slidingRemaining
            ? absoluteRemaining
            : slidingRemaining;
    }
}
