namespace ECommerce.Infrastructure.Caching;

public interface ICachedAggregateStore<T> where T : class
{
    Task<T?> GetAsync(string key, CancellationToken cancellationToken);

    Task<T> GetOrCreateAsync(string key, 
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default);

    Task SetAsync(string key, T value, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken ct = default);
}
