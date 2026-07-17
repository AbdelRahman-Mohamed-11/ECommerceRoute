using ECommerce.API.Extensions;
using ECommerce.API.Models;
using ECommerce.API.Models.Requests;
using ECommerce.UseCases.Basket.Commands;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Basket.Queries;
using ECommerce.UseCases.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.API.Controllers;

[EnableRateLimiting("basket-policy")]
public class BasketController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetBasketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<GetBasketResponse>>> Get(CancellationToken ct = default)
    {
        var buyerIdResult = this.GetBuyerId();
        if (buyerIdResult.IsFailure)
            return Problem(buyerIdResult);

        var result = await sender.Send(new GetBasketQuery(buyerIdResult.Value), ct);
        return FromResult(result, ApiMessages.BasketRetrieved);
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<GetBasketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetBasketResponse>>> AddItem(
        [FromBody] AddBasketItemRequest request,
        CancellationToken ct = default)
    {
        var buyerIdResult = this.GetBuyerId();
        if (buyerIdResult.IsFailure)
            return Problem(buyerIdResult);

        var result = await sender.Send(
            new AddBasketItemCommand(buyerIdResult.Value, request.ProductId, request.Quantity),
            ct);

        return FromResult(result, ApiMessages.BasketItemAdded);
    }

    [HttpPut("items/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetBasketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetBasketResponse>>> UpdateItemQuantity(
        Guid productId,
        [FromBody] UpdateBasketItemQuantityRequest request,
        CancellationToken ct = default)
    {
        var buyerIdResult = this.GetBuyerId();
        if (buyerIdResult.IsFailure)
            return Problem(buyerIdResult);

        var result = await sender.Send(
            new UpdateBasketItemQuantityCommand(buyerIdResult.Value, productId, request.Quantity),
            ct);

        return FromResult(result, ApiMessages.BasketItemUpdated);
    }

    [HttpDelete("items/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetBasketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetBasketResponse>>> RemoveItem(
        Guid productId,
        CancellationToken ct = default)
    {
        var buyerIdResult = this.GetBuyerId();
        if (buyerIdResult.IsFailure)
            return Problem(buyerIdResult);

        var result = await sender.Send(
            new RemoveBasketItemCommand(buyerIdResult.Value, productId),
            ct);

        return FromResult(result, ApiMessages.BasketItemRemoved);
    }

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<GetBasketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<GetBasketResponse>>> Clear(CancellationToken ct = default)
    {
        var buyerIdResult = this.GetBuyerId();
        if (buyerIdResult.IsFailure)
            return Problem(buyerIdResult);

        var result = await sender.Send(new ClearBasketCommand(buyerIdResult.Value), ct);
        return FromResult(result, ApiMessages.BasketCleared);
    }

    [HttpPost("merge")]
    [ProducesResponseType(typeof(ApiResponse<GetBasketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetBasketResponse>>> Merge(
        [FromBody] MergeBasketRequest request,
        CancellationToken ct = default)
    {
        var buyerIdResult = this.GetBuyerId();
        if (buyerIdResult.IsFailure)
            return Problem(buyerIdResult);

        var result = await sender.Send(
            new MergeBasketCommand(buyerIdResult.Value, request.AnonymousBuyerId),
            ct);

        return FromResult(result, ApiMessages.BasketMerged);
    }
}
