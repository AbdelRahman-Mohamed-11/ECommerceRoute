using Asp.Versioning;
using ECommerce.API;
using ECommerce.API.Endpoints;
using ECommerce.API.Test;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using ECommerce.UseCases.Common.Interfaces;
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

    builder.Services.AddPresentation(builder.Configuration);

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();

    app.UseOutputCache();

    app.UseAuthentication();

    app.UseAuthorization();

    var apiVersionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .ReportApiVersions()
        .Build();

    app.MapProductEndpoints(apiVersionSet);
    app.MapTypeEndpoints(apiVersionSet);
    app.MapBrandEndpoints(apiVersionSet);
    app.MapBasketEndpoints(apiVersionSet);

    if (app.Environment.IsDevelopment())
    {
        app.MapPost("/api/test/jwt", (
            GenerateTestJwtRequest request,
            IJwtTokenGenerator jwtTokenGenerator) =>
        {
            var token = jwtTokenGenerator.GenerateToken(
                request.UserId,
                request.Email,
                request.displayName,
                request.Roles);

            return Results.Ok(new
            {
                accessToken = token,
                expiresAt = token.ExpriesAtUtc
            });
        })
        .WithTags("Test")
        .WithSummary("Generates a test JWT")
        .AllowAnonymous();

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
