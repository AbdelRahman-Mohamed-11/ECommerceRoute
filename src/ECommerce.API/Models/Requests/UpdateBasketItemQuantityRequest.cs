using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Entities;

namespace ECommerce.API.Models.Requests;

public sealed class UpdateBasketItemQuantityRequest
{
    [Range(BasketItem.MinQuantity, BasketItem.MaxQuantity)]
    public int Quantity { get; init; }
}
