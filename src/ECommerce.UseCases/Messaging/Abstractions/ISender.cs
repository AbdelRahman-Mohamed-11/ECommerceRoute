namespace ECommerce.UseCases.Messaging;

/// <summary>
/// Dispatches requests to their registered handlers. Application-layer entry point for CQRS.
/// </summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}
