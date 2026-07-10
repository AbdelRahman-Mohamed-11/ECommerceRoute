using System.Linq.Expressions;
using ECommerce.Domain.Specifications;

namespace ECommerce.UseCases.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, bool>>> _whereExpressions = [];
    private readonly List<Expression<Func<T, object>>> _includes = [];
    private readonly List<IncludeExpressionInfo> _includeExpressions = [];
    private readonly List<OrderExpressionInfo<T>> _orderExpressions = [];

    protected ISpecificationBuilder<T> Query => new SpecificationBuilder<T>(this);

    public IReadOnlyList<Expression<Func<T, bool>>> WhereExpressions => _whereExpressions;
    public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes;
    public IReadOnlyList<IncludeExpressionInfo> IncludeExpressions => _includeExpressions;
    public IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions => _orderExpressions;
    public int? Skip { get; private set; }
    public int? Take { get; private set; }
    public bool IsPagingEnabled => Skip.HasValue || Take.HasValue;
    public bool IsTrackingEnabled { get; private set; }

    internal Expression<Func<T, object>> AddInclude<TProperty>(Expression<Func<T, TProperty>> includeExpression)
    {
        // _includes stores Expression<Func<T, object>>, but Include(...) passes
        // Expression<Func<T, TProperty>> (e.g. Expression<Func<Product, ProductBrand>>).
        // Those are different types, so the expression must be re-wrapped:
        //   - Body: the same navigation access (p => p.ProductBrand keeps "p.ProductBrand")
        //   - Parameters: the same lambda parameter ("p")
        // Only the declared return type changes to object; EF still reads the
        // original body to find the navigation, so Include works unchanged.
        // Returned so builders/evaluator share the same expression reference for chain tracking.
        Expression<Func<T, object>> lambda = Expression.Lambda<Func<T, object>>(
            includeExpression.Body,
            includeExpression.Parameters);
        _includes.Add(lambda);
        return lambda;
    }

    internal void AddThenInclude(LambdaExpression navigation, LambdaExpression parent) =>
        _includeExpressions.Add(new IncludeExpressionInfo(navigation, parent));
    internal void AddWhere(Expression<Func<T, bool>> predicate) => _whereExpressions.Add(predicate);
    internal void AddOrder(OrderExpressionInfo<T> info) => _orderExpressions.Add(info);
    internal void SetSkip(int skip) => Skip = skip;
    internal void SetTake(int take) => Take = take;
    internal void SetTracking(bool isTracking) => IsTrackingEnabled = isTracking;
}

public abstract class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
{
    protected new ISpecificationBuilder<T, TResult> Query => new SpecificationBuilder<T, TResult>(this);

    public Expression<Func<T, TResult>>? Selector { get; private set; }
    public Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; private set; }

    internal void SetSelector(Expression<Func<T, TResult>> selector) => Selector = selector;
    internal void SetSelectorMany(Expression<Func<T, IEnumerable<TResult>>> selector) => SelectorMany = selector;
}
