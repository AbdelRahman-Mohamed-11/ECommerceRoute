using ECommerce.Domain.Entities;
using ECommerce.Domain.Specifications;

namespace ECommerce.Domain.Repositories;

// IReadRepository<T> — Domain read port.
// Responsibility: read methods accept ISpecification<T> instead of raw LINQ — Infrastructure applies the spec.
// T = entity type (must inherit BaseEntity).
public interface IReadRepository<T> where T : BaseEntity
{
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default);

    Task<T> SingleAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<TResult> SingleAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default);

    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default);

    Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken ct = default);

    Task<PagedResult<T>> PagedListAsync(ISpecification<T> specification, CancellationToken ct = default);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
