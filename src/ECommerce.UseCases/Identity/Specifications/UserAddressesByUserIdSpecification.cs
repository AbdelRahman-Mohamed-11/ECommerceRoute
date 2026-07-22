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
            .Where(address => address.UserId == userId)
            .Select(address => new UserAddressResponse(
                address.Id,
                address.RecipientFirstName,
                address.RecipientLastName,
                address.PhoneNumber,
                address.Country,
                address.City,
                address.Street,
                address.PostalCode,
                address.IsDefault));
    }
}
