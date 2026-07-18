using Asp.Versioning;
using ECommerce.API;
using ECommerce.API.Endpoints;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    // serliog + seq
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
        var sp = scope.ServiceProvider;

        var identityDb = sp.GetRequiredService<AppIdentityDbContext>();
        await identityDb.Database.MigrateAsync();

        var appDb = sp.GetRequiredService<ApplicationDbContext>();
        await appDb.Database.MigrateAsync();

        await sp.GetRequiredService<DatabaseSeeder>().SeedAll();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    var apiVersionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .ReportApiVersions()
        .Build();

    app.MapAuthEndpoints(apiVersionSet);
    app.MapUserEndpoints(apiVersionSet);
    app.MapProductEndpoints(apiVersionSet);
    app.MapTypeEndpoints(apiVersionSet);
    app.MapBrandEndpoints(apiVersionSet);
    app.MapBasketEndpoints(apiVersionSet);

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
