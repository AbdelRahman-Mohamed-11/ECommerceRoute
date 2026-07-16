using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerce.API.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static Task WriteResponse(HttpContext context, HealthReport report, IHostEnvironment environment)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            TotalDuration = FormatDuration(report.TotalDuration),
            Checks = report.Entries.Select(entry => new HealthCheckEntry
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description,
                Duration = FormatDuration(entry.Value.Duration),
                Exception = environment.IsDevelopment()
                    ? entry.Value.Exception?.Message
                    : null
            }).ToList()
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, SerializerOptions));
    }

    private static string FormatDuration(TimeSpan duration) =>
        duration.TotalMilliseconds < 1000
            ? $"{duration.TotalMilliseconds:F1}ms"
            : $"{duration.TotalSeconds:F2}s";
}

public sealed class HealthCheckResponse
{
    public required string Status { get; init; }
    public required string TotalDuration { get; init; }
    public required List<HealthCheckEntry> Checks { get; init; }
}

public sealed class HealthCheckEntry
{
    public required string Name { get; init; }
    public required string Status { get; init; }
    public string? Description { get; init; }
    public required string Duration { get; init; }
    public string? Exception { get; init; }
}
