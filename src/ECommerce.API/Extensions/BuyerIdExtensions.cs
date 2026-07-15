using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Extensions;

public static class BuyerIdExtensions
{
    public const string HeaderName = "X-Buyer-Id";

    public static Result<Guid> GetBuyerId(this ControllerBase controller)
    {
        if (!controller.Request.Headers.TryGetValue(HeaderName, out var headerValue))
            return Result<Guid>.Failure(BasketErrors.GuestBuyerIdRequired);

        if (!Guid.TryParse(headerValue, out var buyerId) || buyerId == Guid.Empty)
            return Result<Guid>.Failure(BasketErrors.InvalidBuyerId);

        // read from jwt

        return buyerId;
    }
}
