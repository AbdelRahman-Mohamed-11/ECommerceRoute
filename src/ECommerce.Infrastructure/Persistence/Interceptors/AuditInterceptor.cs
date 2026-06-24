using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Interceptors;

public sealed class AuditInterceptor : IAuditInterceptor
{
    public void Apply(DbContext dbContext)
    {
        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in dbContext.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.Id == Guid.Empty)
                    {
                        entry.Property(nameof(BaseEntity.Id)).CurrentValue = Guid.NewGuid();
                    }

                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = utcNow;
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = null;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = utcNow;
                    break;
            }
        }
    }
}
