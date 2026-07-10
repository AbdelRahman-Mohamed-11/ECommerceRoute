using ECommerce.UseCases.Products.Queries;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (GetAllProductsQuery getAllProductsQuery, CancellationToken cancellationToken) =>
        {
            var result = await getAllProductsQuery.ExecuteAsync(cancellationToken);

            return result.IsFailure
                ? Results.Problem(result.Error.Message)
                : Results.Ok(result.Value);
        });

        return endpoints;
    }
}
