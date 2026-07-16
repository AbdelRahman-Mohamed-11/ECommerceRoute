using ECommerce.Infrastructure.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.Infrastructure.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();

        builder.AddCheck<DatabaseHealthCheck>(
            "database",
            tags: ["ready", "database"]);

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            builder.AddRedis(
                redisConnection,
                name: "redis",
                tags: ["ready", "redis"]);
        }

        builder.AddCheck(
            "liveness",
            () => HealthCheckResult.Healthy("Application is running."),
            tags: ["live"]);

        return services;
    }
}
