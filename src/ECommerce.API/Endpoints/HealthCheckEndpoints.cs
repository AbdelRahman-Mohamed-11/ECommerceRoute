using ECommerce.API.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ECommerce.API.Endpoints;

public static class HealthCheckEndpoints
{
    public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var environment = endpoints.ServiceProvider.GetRequiredService<IHostEnvironment>();

        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = (context, report) =>
                HealthCheckResponseWriter.WriteResponse(context, report, environment)
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = (context, report) =>
                HealthCheckResponseWriter.WriteResponse(context, report, environment)
        });

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = (context, report) =>
                HealthCheckResponseWriter.WriteResponse(context, report, environment)
        });

        return endpoints;
    }
}
