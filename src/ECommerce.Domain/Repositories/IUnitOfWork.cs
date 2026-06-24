using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories;

/// <summary>
/// Coordinates one or more repositories sharing a single DbContext / transaction.
/// SaveChangesAsync is the only place a transaction is committed — repositories
/// never call SaveChanges themselves.
/// </summary>
public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken ct = default); 
}