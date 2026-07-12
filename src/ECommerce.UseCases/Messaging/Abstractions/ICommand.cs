namespace ECommerce.UseCases.Messaging;

/// <summary>Semantic marker for write/mutation requests.</summary>
public interface ICommand<out TResponse> : IRequest<TResponse>;
