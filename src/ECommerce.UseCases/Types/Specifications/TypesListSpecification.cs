using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;
using ECommerce.UseCases.Types.Dtos;

namespace ECommerce.UseCases.Types.Specifications;

public sealed class TypesListSpecification : Specification<ProductType, GetAllTypesResponse>
{
    public TypesListSpecification()
    {
        Query
            .OrderBy(type => type.Name)
            .Select(type => new GetAllTypesResponse(type.Id, type.Name));
    }
}
