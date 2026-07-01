using ECommerce.UseCases.Brands.Queries;
using ECommerce.UseCases.Products.Queries;
using ECommerce.UseCases.Types.Queries;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MappingConfig).Assembly);

        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<GetAllProductsQuery>();
        services.AddScoped<GetByIdProductQuery>();
        services.AddScoped<GetAllBrandsQuery>();
        services.AddScoped<GetAllTypesQuery>();

        return services;
    }
}
