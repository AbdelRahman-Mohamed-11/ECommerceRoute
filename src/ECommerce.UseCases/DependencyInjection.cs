using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Queries.GetPagedProducts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMessaging(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(typeof(GetPagedProductsQueryValidator).Assembly);

        return services;
    }
}
