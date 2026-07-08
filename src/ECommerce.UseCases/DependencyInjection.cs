using ECommerce.UseCases.Brands.Queries;
using ECommerce.UseCases.Products.Queries;
using ECommerce.UseCases.Types.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetAllProductsQuery>();
        services.AddScoped<GetByIdProductQuery>();
        services.AddScoped<GetAllBrandsQuery>();
        services.AddScoped<GetAllTypesQuery>();

        return services;
    }
}
