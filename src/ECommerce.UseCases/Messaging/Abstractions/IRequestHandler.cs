namespace ECommerce.UseCases.Messaging;

/// <summary>
/// Handles a single request type. Register implementations via assembly scanning in DI.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
