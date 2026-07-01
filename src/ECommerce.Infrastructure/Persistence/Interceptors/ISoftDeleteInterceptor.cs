using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Interceptors;

public interface ISoftDeleteInterceptor
{
    void Apply(DbContext dbContext);
}
