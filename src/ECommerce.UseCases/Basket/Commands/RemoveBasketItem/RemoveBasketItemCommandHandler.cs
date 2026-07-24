using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.RemoveBasketItem;

public sealed class RemoveBasketItemCommandHandler(IBasketStore basketStore)
    : IRequestHandler<RemoveBasketItemCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(
        RemoveBasketItemCommand request,
        CancellationToken cancellationToken)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var removeResult = basket.RemoveItem(request.ProductId);
        if (removeResult.IsFailure)
            return Result<GetBasketResponse>.Failure(removeResult.Error);

        await basketStore.SaveAsync(basket, cancellationToken);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}