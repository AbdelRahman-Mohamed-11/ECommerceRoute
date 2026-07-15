using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.Requests;

public sealed class MergeBasketRequest
{
    [Required]
    public Guid AnonymousBuyerId { get; init; }
}
