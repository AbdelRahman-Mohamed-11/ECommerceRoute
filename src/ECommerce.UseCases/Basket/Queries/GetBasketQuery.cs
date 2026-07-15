using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Queries;

public sealed record GetBasketQuery(Guid BuyerId) : IQuery<Result<GetBasketResponse>>;

public sealed class GetBasketQueryHandler(IBasketStore basketStore)
    : IRequestHandler<GetBasketQuery, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(
        GetBasketQuery request,
        CancellationToken cancellationToken)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}
