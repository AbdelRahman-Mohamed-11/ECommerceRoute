using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Queries;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllProductsQuery(), cancellationToken);

            return result.IsFailure
                ? Results.Problem(result.Error.Message)
                : Results.Ok(result.Value);
        });

        return endpoints;
    }
}
