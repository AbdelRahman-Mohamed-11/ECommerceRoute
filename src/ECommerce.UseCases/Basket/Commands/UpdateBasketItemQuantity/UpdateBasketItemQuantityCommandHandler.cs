using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.UpdateBasketItemQuantity;

public sealed class UpdateBasketItemQuantityCommandHandler(IBasketStore basketStore)
    : IRequestHandler<UpdateBasketItemQuantityCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(
        UpdateBasketItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var updateResult = basket.UpdateItemQuantity(request.ProductId, request.Quantity);
        if (updateResult.IsFailure)
            return Result<GetBasketResponse>.Failure(updateResult.Error);

        await basketStore.SaveAsync(basket, cancellationToken);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}