using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Interceptors;

public sealed class SoftDeleteInterceptor : ISoftDeleteInterceptor
{
    public void Apply(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.MarkAsDeleted();
        }
    }
}
