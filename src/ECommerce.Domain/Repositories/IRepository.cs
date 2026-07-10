using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories;

// IRepository<T> — Domain write port; extends read repository with Add / Update / Delete.
public interface IRepository<T> : IReadRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}
