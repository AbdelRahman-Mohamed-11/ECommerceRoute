namespace ECommerce.UseCases.Messaging;

internal interface IRequestHandlerExecutor
{
    Type RequestType { get; }

    Task<object?> ExecuteAsync(object request, CancellationToken cancellationToken);
}
