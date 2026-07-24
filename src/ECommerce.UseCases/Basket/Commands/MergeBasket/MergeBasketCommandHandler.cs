using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.MergeBasket;

public sealed class MergeBasketCommandHandler(IBasketStore basketStore)
    : IRequestHandler<MergeBasketCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(
        MergeBasketCommand request,
        CancellationToken cancellationToken)
    {
        if (request.AnonymousBuyerId == Guid.Empty)
            return Result<GetBasketResponse>.Failure(BasketErrors.AnonymousBuyerRequired);

        var anonymousBasket = await basketStore.GetAsync(request.AnonymousBuyerId, cancellationToken);
        if (anonymousBasket is null || anonymousBasket.Items.Count == 0)
            return Result<GetBasketResponse>.Failure(BasketErrors.AnonymousBasketNotFound);

        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var mergeResult = basket.MergeFrom(anonymousBasket);
        if (mergeResult.IsFailure)
            return Result<GetBasketResponse>.Failure(mergeResult.Error);

        await basketStore.SaveAsync(basket, cancellationToken);
        await basketStore.DeleteAsync(request.AnonymousBuyerId, cancellationToken);

        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}