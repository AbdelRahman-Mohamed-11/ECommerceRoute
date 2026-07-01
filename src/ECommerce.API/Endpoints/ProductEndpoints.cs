using ECommerce.UseCases.Products;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (IProductQueryService productQueryService, CancellationToken cancellationToken) =>
        {
            var products = await productQueryService.GetAllProductsAsync(cancellationToken);
            return Results.Ok(products); // 200
        });
        
        return endpoints;
    }
}