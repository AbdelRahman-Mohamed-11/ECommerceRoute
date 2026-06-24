using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Interceptors;

public interface IAuditInterceptor
{
    void Apply(DbContext dbContext);
}
