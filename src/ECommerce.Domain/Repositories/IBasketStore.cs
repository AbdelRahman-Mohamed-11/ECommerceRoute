using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories;

public interface IBasketStore
{
    Task<Basket?> GetAsync(Guid buyerId, CancellationToken ct = default);

    Task<Basket> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default);

    Task SaveAsync(Basket basket, CancellationToken ct = default);

    Task DeleteAsync(Guid buyerId, CancellationToken ct = default);
}
