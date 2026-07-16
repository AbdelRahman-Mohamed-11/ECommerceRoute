using ECommerce.Infrastructure.Persistence.DbContexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.HealthChecks;

public sealed class DatabaseHealthCheck(
    StoreDbContext dbContext,
    ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                logger.LogWarning("Database health check failed: unable to establish a connection.");
                return HealthCheckResult.Unhealthy("Unable to connect to the database.");
            }

            return HealthCheckResult.Healthy("Database is reachable.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database health check threw an exception.");
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                description: "Database health check failed with an exception.",
                exception: ex);
        }
    }
}
