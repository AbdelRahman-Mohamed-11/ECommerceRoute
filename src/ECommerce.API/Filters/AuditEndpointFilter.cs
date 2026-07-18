namespace ECommerce.API.Filters;

public class AuditEndpointFilter(
    ILogger<AuditEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        var userId = httpContext.User.FindFirst("sub")?.Value;
        var endpointName = httpContext.GetEndpoint()?.DisplayName;

        var result = await next(context);

        var statusCode = result switch
        {
            IStatusCodeHttpResult statusResult => statusResult.StatusCode,
            _ => httpContext.Response.StatusCode
        };

        logger.LogInformation(
            "User {UserId} executed {EndpointName} with Status Code {StatusCode}",
            userId,
            endpointName,
            statusCode);

        return result;
    }
}
