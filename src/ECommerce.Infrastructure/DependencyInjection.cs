using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Interceptors;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"), sql => 
            sql.MigrationsHistoryTable("__ApplicationMigrationsHistory"))
                   .EnableSensitiveDataLogging();
        });

        services.AddDbContext<IdentityStoreDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"), sql =>
            sql.MigrationsHistoryTable("__IdentityMigrationsHistory"))
                   .EnableSensitiveDataLogging();
        });

        services.AddScoped<IAuditInterceptor, AuditInterceptor>();
        services.AddScoped<ISoftDeleteInterceptor, SoftDeleteInterceptor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();
        services.AddScoped<IDataSeeder, IdentitySeeder>();

        services.AddScoped<DatabaseSeeder>();

        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IAttachmentService, AttachmentService>();

        AddBasketCaching(services, config);

        return services;
    }

    private static void AddBasketCaching(IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<CacheEntryPolicy>("Basket")
            .Bind(config.GetSection("CachedAggregates:Basket"))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<CacheEntryPolicy>, CacheEntryPolicyValidator>();

        var redisConnection = config.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = redisConnection);
        }
      

        services.AddHybridCache();

        services.AddScoped(typeof(ICachedAggregateStore<>), typeof(HybridCacheAggregateStore<>));
        services.AddScoped<IBasketStore, HybridBasketStore>();
    }
}
