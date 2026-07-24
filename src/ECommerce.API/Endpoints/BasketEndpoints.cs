using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.API.Models.Requests;
using ECommerce.UseCases.Basket.Commands.AddBasketItem;
using ECommerce.UseCases.Basket.Commands.ClearBasket;
using ECommerce.UseCases.Basket.Commands.MergeBasket;
using ECommerce.UseCases.Basket.Commands.RemoveBasketItem;
using ECommerce.UseCases.Basket.Commands.UpdateBasketItemQuantity;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Basket.Queries.GetBasket;
using ECommerce.UseCases.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class BasketEndpoints
{
    public static IEndpointRouteBuilder MapBasketEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/basket")
            .WithTags("Basket")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapGet("/", async (
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var buyerIdResult = httpContext.GetBuyerId();

            if (buyerIdResult.IsFailure)
                return buyerIdResult.Problem(httpContext);

            var result = await sender.Send(new GetBasketQuery(buyerIdResult.Value), ct);

            return result.FromResult(httpContext, ApiMessages.BasketRetrieved);
        })
        .WithSummary("Gets the basket by user ID")
        .WithDescription("Returns the basket for the specified user ID")
        .Produces<ApiResponse<GetBasketResponse>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/items", async (
            [FromBody] AddBasketItemRequest request,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var buyerIdResult = httpContext.GetBuyerId();

            if (buyerIdResult.IsFailure)
                return buyerIdResult.Problem(httpContext);

            var result = await sender.Send(
                new AddBasketItemCommand(buyerIdResult.Value, request.ProductId, request.Quantity),
                ct);

            return result.FromResult(httpContext, ApiMessages.BasketItemAdded);
        })
        .WithSummary("Add item in the basket")
        .WithDescription("Returns the created item in the basket for the specified user ID")
        .Produces<ApiResponse<GetBasketResponse>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        group.MapPut("/items/{productId:guid}", async (
            Guid productId,
            [FromBody] UpdateBasketItemQuantityRequest request,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var buyerIdResult = httpContext.GetBuyerId();

            if (buyerIdResult.IsFailure)
                return buyerIdResult.Problem(httpContext);

            var result = await sender.Send(
                new UpdateBasketItemQuantityCommand(buyerIdResult.Value, productId, request.Quantity),
                ct);

            return result.FromResult(httpContext, ApiMessages.BasketItemUpdated);
        })
        .WithSummary("Update the item inside the basket")
        .WithDescription("Returns the updated basket for the specified user ID")
        .Produces<ApiResponse<GetBasketResponse>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        group.MapDelete("/items/{productId:guid}", async (
            Guid productId,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var buyerIdResult = httpContext.GetBuyerId();

            if (buyerIdResult.IsFailure)
                return buyerIdResult.Problem(httpContext);

            var result = await sender.Send(
                new RemoveBasketItemCommand(buyerIdResult.Value, productId),
                ct);

            return result.FromResult(httpContext, ApiMessages.BasketItemRemoved);
        })
        .WithSummary("Remove the item from the basket")
        .WithDescription("Returns the updated basket for the specified user ID")
        .Produces<ApiResponse<GetBasketResponse>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        group.MapDelete("/", async (
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var buyerIdResult = httpContext.GetBuyerId();

            if (buyerIdResult.IsFailure)
                return buyerIdResult.Problem(httpContext);

            var result = await sender.Send(
                new ClearBasketCommand(buyerIdResult.Value),
                ct);

            return result.FromResult(httpContext, ApiMessages.BasketCleared);
        })
        .WithSummary("Clear the basket")
        .WithDescription("Returns the cleared basket for the specified user ID")
        .Produces<ApiResponse<GetBasketResponse>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/merge", async (
            [FromBody] MergeBasketRequest request,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var buyerIdResult = httpContext.GetBuyerId();

            if (buyerIdResult.IsFailure)
                return buyerIdResult.Problem(httpContext);

            var result = await sender.Send(
                new MergeBasketCommand(buyerIdResult.Value, request.AnonymousBuyerId),
                ct);

            return result.FromResult(httpContext, ApiMessages.BasketMerged);
        })
        .WithSummary("Merge the baskets together")
        .WithDescription("Returns the updated basket for the specified user ID")
        .Produces<ApiResponse<GetBasketResponse>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
