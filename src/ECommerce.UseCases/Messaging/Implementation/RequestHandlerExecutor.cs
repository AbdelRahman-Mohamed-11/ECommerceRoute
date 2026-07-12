namespace ECommerce.UseCases.Messaging;

internal sealed class RequestHandlerExecutor<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> handler) : IRequestHandlerExecutor
    where TRequest : IRequest<TResponse>
{
    public Type RequestType => typeof(TRequest);

    public async Task<object?> ExecuteAsync(object request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not TRequest typedRequest)
        {
            throw new InvalidOperationException(
                $"Expected request of type {typeof(TRequest).Name}, but received {request.GetType().Name}.");
        }

        return await handler.Handle(typedRequest, cancellationToken).ConfigureAwait(false);
    }
}
