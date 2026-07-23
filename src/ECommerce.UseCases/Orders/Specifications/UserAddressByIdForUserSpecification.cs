using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Orders.Specifications;

public sealed class UserAddressByIdForUserSpecification : Specification<UserAddress>
{
    public UserAddressByIdForUserSpecification(Guid addressId, Guid userId)
    {
        Query.Where(a => a.Id == addressId && a.UserId == userId);
    }
}
