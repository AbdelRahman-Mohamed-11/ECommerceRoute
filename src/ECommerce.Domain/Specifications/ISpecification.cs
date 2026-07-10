using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

public interface ISpecification<T>
{
    // Filter predicates — combined with AND in the evaluator
    IReadOnlyList<Expression<Func<T, bool>>> WhereExpressions { get; }

    // EF Include lambdas — Expression<Func<T, object>>:
    //   object = common EF trick so one type accepts any property (reference or boxed value)
    //   e.g. p => p.ProductBrand, p => p.ProductType
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }

    // ThenInclude chains — each entry extends a root Include (see IncludeExpressionInfo)
    IReadOnlyList<IncludeExpressionInfo> IncludeExpressions { get; }

    // Sort rules in apply order (OrderBy, ThenBy, …)
    IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions { get; }

    // Pagination — Skip/Take applied after filters and ordering
    int? Skip { get; }
    int? Take { get; }
    bool IsPagingEnabled { get; }  // true when Skip or Take is set

    // false = AsNoTracking(), true = AsTracking()
    bool IsTrackingEnabled { get; }
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Selector { get; }
    Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; }
}
