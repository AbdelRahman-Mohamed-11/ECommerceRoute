using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Types.Dtos;
using ECommerce.UseCases.Types.Queries.GetAllTypes;

namespace ECommerce.API.Endpoints;

public static class TypeEndpoints
{
    public static IEndpointRouteBuilder MapTypeEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/types")
            .WithTags("Types")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapGet("/", async (
            ISender sender,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllTypesQuery(), cancellationToken);
            return result.FromResult(httpContext, "Types retrieved successfully");
        })
        .WithSummary("Gets all types")
        .WithDescription("Returns a list of all types")
        .Produces<ApiResponse<IReadOnlyList<GetAllTypesResponse>>>(StatusCodes.Status200OK);

        return endpoints;
    }
}
