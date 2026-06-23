using ECommerce.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                   .EnableSensitiveDataLogging();
        });

        return services;
    }
}
