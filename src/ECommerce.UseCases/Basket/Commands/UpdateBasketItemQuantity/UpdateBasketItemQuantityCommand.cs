using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Basket.Commands.UpdateBasketItemQuantity;

public sealed record UpdateBasketItemQuantityCommand(Guid BuyerId, Guid ProductId, int Quantity)
    : ICommand<Result<GetBasketResponse>>;