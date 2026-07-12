using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases.Messaging;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        RegisterHandlers(services, assembly);

        services.AddScoped<HandlerExecutorRegistry>();
        services.AddScoped<ISender, Sender>();

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var handlerInterface = typeof(IRequestHandler<,>);

        foreach (var handlerType in assembly.GetTypes())
        {
            if (handlerType is not { IsClass: true, IsAbstract: false })
                continue;

            foreach (var serviceInterface in handlerType.GetInterfaces())
            {
                if (!serviceInterface.IsGenericType ||
                    serviceInterface.GetGenericTypeDefinition() != handlerInterface)
                {
                    continue;
                }

                var requestType = serviceInterface.GetGenericArguments()[0];
                var responseType = serviceInterface.GetGenericArguments()[1];

                services.AddScoped(serviceInterface, handlerType);

                var executorType = typeof(RequestHandlerExecutor<,>).MakeGenericType(requestType, responseType);
                services.AddScoped(typeof(IRequestHandlerExecutor), executorType);
            }
        }
    }
}
