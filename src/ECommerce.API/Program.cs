using ECommerce.API;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddOutputCache(options =>
    {
        options.AddPolicy("Products", policy =>
        {
            policy.Expire(TimeSpan.FromMinutes(1))
            .SetVaryByQuery("pagesize", "pageNumber");
        });
    });

    builder.Services.AddPresentation();

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseMetricServer("/metrics");

    app.UseHttpMetrics();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();

    app.UseOutputCache();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

        await using var scope = app.Services.CreateAsyncScope();

        var dbSeed = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

        await dbContext.Database.MigrateAsync();

        await dbSeed.SeedAll();
    }

    app.MapControllers();

    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
