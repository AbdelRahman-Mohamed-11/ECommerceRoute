using System.Security.Claims;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.API.Extensions;

public static class BuyerIdExtensions
{
    public const string HeaderName = "X-Buyer-Id";

    public static Result<Guid> GetBuyerId(this HttpContext context)
    {
        // Authenticated shopper → buyer id comes from the JWT (NameIdentifier / sub).
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId) || userId == Guid.Empty)
                return Result<Guid>.Failure(BasketErrors.AuthenticatedBuyerIdMissing);

            return userId;
        }

        // Guest shopper → client-generated GUID via X-Buyer-Id header.
        return GetGuestBuyerId(context.Request.Headers);
    }

    private static Result<Guid> GetGuestBuyerId(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(HeaderName, out var headerValue))
            return Result<Guid>.Failure(BasketErrors.GuestBuyerIdRequired);

        if (!Guid.TryParse(headerValue, out var buyerId) || buyerId == Guid.Empty)
            return Result<Guid>.Failure(BasketErrors.InvalidBuyerId);

        return buyerId;
    }
}
