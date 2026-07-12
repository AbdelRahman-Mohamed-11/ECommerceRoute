namespace ECommerce.UseCases.Messaging;

/// <summary>Semantic marker for read-only requests.</summary>
public interface IQuery<out TResponse> : IRequest<TResponse>;
