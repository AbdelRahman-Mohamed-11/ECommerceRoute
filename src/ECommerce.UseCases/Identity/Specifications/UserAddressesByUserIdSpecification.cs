using ECommerce.Domain.Entities;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Identity.Specifications;

public sealed class UserAddressesByUserIdSpecification
    : Specification<UserAddress, UserAddressResponse>
{
    public UserAddressesByUserIdSpecification(Guid userId)
    {
        Query
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefaultShipping)
            .ThenBy(a => a.Label)
            .Select(a => new UserAddressResponse(
                a.Id,
                a.Label,
                a.RecipientFirstName,
                a.RecipientLastName,
                a.PhoneNumber,
                a.Country,
                a.City,
                a.Street,
                a.PostalCode,
                a.IsDefaultShipping,
                a.IsDefaultBilling));
    }
}
