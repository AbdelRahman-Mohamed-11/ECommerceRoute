using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Queries.GetBasket;

public sealed record GetBasketQuery(Guid BuyerId) : IQuery<Result<GetBasketResponse>>;