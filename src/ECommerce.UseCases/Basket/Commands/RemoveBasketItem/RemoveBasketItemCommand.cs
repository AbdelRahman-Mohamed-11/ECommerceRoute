using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.RemoveBasketItem;

public sealed record RemoveBasketItemCommand(Guid BuyerId, Guid ProductId)
    : ICommand<Result<GetBasketResponse>>;