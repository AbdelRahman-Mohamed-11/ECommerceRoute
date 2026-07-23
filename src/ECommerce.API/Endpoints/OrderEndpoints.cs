using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Commands.CancelOrder;
using ECommerce.UseCases.Orders.Commands.CreateOrder;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Queries.GetMyOrders;
using ECommerce.UseCases.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/orders")
            .WithTags("Orders")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .RequireAuthorization()
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapPost("/", async (
            [FromBody] CreateOrderCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.OrderCreated);
        })
        .WithSummary("Create order (checkout)")
        .WithDescription("Creates an order from the authenticated user's basket, then clears the basket.")
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/", async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetMyOrdersQuery(
                    pageNumber <= 0 ? 1 : pageNumber,
                    pageSize <= 0 ? 10 : pageSize),
                ct);

            return result.FromPagedResult(
                httpContext,
                pageNumber <= 0 ? 1 : pageNumber,
                pageSize <= 0 ? 10 : pageSize,
                ApiMessages.OrdersRetrieved);
        })
        .WithSummary("Get current user orders")
        .Produces<ApiResponse<IReadOnlyList<OrderResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetOrderByIdQuery(id), ct);
            return result.FromResult(httpContext, ApiMessages.OrderRetrieved);
        })
        .WithSummary("Get order by id")
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new CancelOrderCommand(id), ct);
            return result.FromResult(httpContext, ApiMessages.OrderCancelled);
        })
        .WithSummary("Cancel pending order")
        .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
