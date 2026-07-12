namespace ECommerce.UseCases.Messaging;

/// <summary>
/// Marker for a request that returns <typeparamref name="TResponse"/>.
/// Implemented by commands and queries; dispatched via <see cref="ISender"/>.
/// </summary>
public interface IRequest<out TResponse>;
