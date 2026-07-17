using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ECommerce.API.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
           
            options.AddPolicy("products-policy", httpContext =>
            {
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 60,
                    QueueLimit = 5,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            
            options.AddPolicy("basket-policy", httpContext =>
            {
                string buyerId = "anonymous";
                if (httpContext.Request.Headers.TryGetValue(BuyerIdExtensions.HeaderName, out var headerValue))
                {
                    buyerId = headerValue.ToString();
                }

                return RateLimitPartition.GetFixedWindowLimiter(buyerId, _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 10,
                    QueueLimit = 2,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

           
            options.AddPolicy("upload-policy", httpContext =>
            {
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 5,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(5),
                    TokensPerPeriod = 5,
                    AutoReplenishment = true
                });
            });

          
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

           
           options.OnRejected = async (context, token) =>
           {
               
               if (context.Lease.TryGetMetadata(
                       MetadataName.RetryAfter,
                       out var retryAfter))
               {
                   context.HttpContext.Response.Headers.RetryAfter =
                       ((int)retryAfter.TotalSeconds).ToString();
               }

               context.HttpContext.Response.ContentType = "application/json";

               var problem = new Dictionary<string, object?>
               {
                   ["type"] = "https://example.com/errors/too-many-requests",
                   ["title"] = "Too Many Requests",
                   ["status"] = StatusCodes.Status429TooManyRequests,
                   ["detail"] = "You have exceeded the allowed request limit. Please wait and try again later.",
                   ["traceId"] = context.HttpContext.TraceIdentifier
               };

               await context.HttpContext.Response.WriteAsJsonAsync(problem, cancellationToken: token);
           };

        });

        return services;
    }
}
