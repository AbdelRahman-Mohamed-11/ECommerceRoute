namespace ECommerce.UseCases.Messaging;

internal sealed class HandlerExecutorRegistry
{
    private readonly IReadOnlyDictionary<Type, IRequestHandlerExecutor> _executors;

    public HandlerExecutorRegistry(IEnumerable<IRequestHandlerExecutor> executors)
    {
        ArgumentNullException.ThrowIfNull(executors);

        _executors = executors.ToDictionary(executor => executor.RequestType);
    }

    public IRequestHandlerExecutor GetRequiredExecutor(Type requestType)
    {
        ArgumentNullException.ThrowIfNull(requestType);

        if (_executors.TryGetValue(requestType, out var executor))
            return executor;

        throw new InvalidOperationException(
            $"No handler registered for request type '{requestType.FullName}'.");
    }
}
