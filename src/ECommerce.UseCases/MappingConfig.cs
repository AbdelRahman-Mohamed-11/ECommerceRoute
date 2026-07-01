using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.Dtos;
using Mapster;

namespace ECommerce.UseCases;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, GetByIdProductResponse>()
            .Map(dest => dest.ProductBrand, src => src.ProductBrand.Name)
            .Map(dest => dest.ProductType, src => src.ProductType.Name);
    }
}
