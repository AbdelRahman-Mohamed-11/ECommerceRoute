using ECommerce.Domain.Specifications;

namespace ECommerce.Infrastructure.Specifications;

// QueryableSpecificationExtensions — Infrastructure extension methods.
// Responsibility: shortcut for query services — dbSet.WithSpecification(spec) → SpecificationEvaluator.
public static class QueryableSpecificationExtensions
{
    public static IQueryable<T> WithSpecification<T>(
        this IQueryable<T> source,
        ISpecification<T> specification,
        bool ignorePaging = false)
        where T : class =>
        SpecificationEvaluator.GetQuery(source, specification, ignorePaging);

    public static IQueryable<TResult> WithSpecification<T, TResult>(
        this IQueryable<T> source,
        ISpecification<T, TResult> specification)
        where T : class =>
        SpecificationEvaluator.GetQuery(source, specification);
}
