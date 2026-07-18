using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Commands.CreateProduct;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/products")
            .WithTags("Products")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapGet("/paged", async (
            [AsParameters] GetPagedProductsQuery query,
            ISender sender,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(query, cancellationToken);

            return result.FromPagedResult(httpContext, query.PageNumber, query.PageSize, "Products retrieved succefully");
        })
        .WithSummary("Gets paginated products")
        .WithDescription("Returns a paginated list of products with filtering and sorting options")
        .CacheOutput("Products")
        .Produces<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetByIdProductQuery(id), cancellationToken);

            return result.FromResult(httpContext, "Product id retrieved succefully");
        })
        .WithName("GetProductById")
        .WithSummary("Gets product by ID")
        .WithDescription("Returns product information")
        .Produces<ApiResponse<GetByIdProductResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (
            [FromForm] CreateProductCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return result.Problem(httpContext);

            return Results.CreatedAtRoute(
                routeName: "GetProductById",
                routeValues: new { id = result.Value },
                value: ApiResponse<Guid>.Ok(
                    result.Value,
                    httpContext.TraceIdentifier,
                    "Product created successfully"));
        })
        .DisableAntiforgery()
        .WithSummary("Create product")
        .WithDescription("Returns product ID")
        .Accepts<CreateProductCommand>("multipart/form-data")
        .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}
