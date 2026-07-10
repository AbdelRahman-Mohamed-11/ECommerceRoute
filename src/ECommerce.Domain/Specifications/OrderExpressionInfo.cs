using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

// orderby(p => p.Price)
public record OrderExpressionInfo<T>(
    Expression<Func<T, object?>> KeySelector,
    OrderType OrderType);


public enum OrderType
{
    OrderBy,
    OrderByDescending,
    ThenBy,
    ThenByDescending,
}

public sealed record OrderExpressionInfo<T>(
    Expression<Func<T, object?>> KeySelector,
    OrderType OrderType);