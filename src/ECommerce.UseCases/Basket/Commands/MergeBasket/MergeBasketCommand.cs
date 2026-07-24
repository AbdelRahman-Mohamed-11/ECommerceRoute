using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.MergeBasket;

public sealed record MergeBasketCommand(Guid BuyerId, Guid AnonymousBuyerId)
    : ICommand<Result<GetBasketResponse>>;