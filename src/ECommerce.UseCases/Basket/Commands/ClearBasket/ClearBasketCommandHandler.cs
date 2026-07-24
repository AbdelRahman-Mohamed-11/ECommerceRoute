using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.ClearBasket;

public sealed class ClearBasketCommandHandler(IBasketStore basketStore)
    : IRequestHandler<ClearBasketCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(
        ClearBasketCommand request,
        CancellationToken cancellationToken)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);
        basket.Clear();
        await basketStore.SaveAsync(basket, cancellationToken);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}