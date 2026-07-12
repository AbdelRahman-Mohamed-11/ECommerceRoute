using System.Reflection;
using ECommerce.UseCases.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMessaging(Assembly.GetExecutingAssembly());

        return services;
    }
}
