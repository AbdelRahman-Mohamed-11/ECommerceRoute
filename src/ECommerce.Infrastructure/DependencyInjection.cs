using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Interceptors;
using ECommerce.Infrastructure.Persistence.Queries;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.Infrastructure.Repositories;
using ECommerce.UseCases.Products;
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

        services.AddScoped<IAuditInterceptor, AuditInterceptor>();
        services.AddScoped<ISoftDeleteInterceptor, SoftDeleteInterceptor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();

        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IProductQueryService, ProductQueryService>();

        return services;
    }
}
