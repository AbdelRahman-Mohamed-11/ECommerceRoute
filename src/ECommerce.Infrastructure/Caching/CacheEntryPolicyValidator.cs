using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Caching;

public sealed class CacheEntryPolicyValidator : IValidateOptions<CacheEntryPolicy>
{
    public ValidateOptionsResult Validate(string? name, CacheEntryPolicy options)
    {
        var failures = new List<string>();

        if (options.AbsoluteExpirationDays <= 0)
            failures.Add($"{nameof(options.AbsoluteExpirationDays)} must be greater than zero.");

        if (options.SlidingExpirationDays <= 0)
            failures.Add($"{nameof(options.SlidingExpirationDays)} must be greater than zero.");

        if (options.LocalCacheExpirationMinutes <= 0)
            failures.Add($"{nameof(options.LocalCacheExpirationMinutes)} must be greater than zero.");

        if (options.SlidingRefreshThresholdMinutes < 0)
            failures.Add($"{nameof(options.SlidingRefreshThresholdMinutes)} cannot be negative.");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
