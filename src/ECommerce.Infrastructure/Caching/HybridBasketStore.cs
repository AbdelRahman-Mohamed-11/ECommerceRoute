using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;

namespace ECommerce.Infrastructure.Caching;

public sealed class HybridBasketStore(ICachedAggregateStore<Basket> store) : IBasketStore
{
    public Task<Basket?> GetAsync(Guid buyerId, CancellationToken ct = default) =>
        store.GetAsync(BuildCacheKey(buyerId), ct);

    public Task<Basket> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default) =>
        store.GetOrCreateAsync(
            BuildCacheKey(buyerId),
            async _ =>
            {
                var createResult = Basket.CreateEmpty(buyerId);

                if (createResult.IsFailure)
                    throw new InvalidOperationException(createResult.Error.Message);

                return createResult.Value;
            },
            ct);

    public Task SaveAsync(Basket basket, CancellationToken ct = default) =>
        store.SetAsync(BuildCacheKey(basket.BuyerId), basket, ct);

    public Task DeleteAsync(Guid buyerId, CancellationToken ct = default) =>
        store.RemoveAsync(BuildCacheKey(buyerId), ct);

    private static string BuildCacheKey(Guid buyerId) => $"basket:{buyerId}";
}
