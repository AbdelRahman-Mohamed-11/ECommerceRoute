using ECommerce.UseCases.Specifications;
using ECommerce.UseCases.Specifications.Includes;
using ECommerce.UseCases.Specifications.OrderInterfaces;
using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications;

public interface ISpecificationBuilder<T>
{
    ISpecificationBuilder<T> Where(Expression<Func<T, bool>> predicate);

    IOrderedSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderExpression);


    IIncludableSpecificationBuilder<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> navigation);
    IIncludableCollectionSpecificationBuilder<T, TElement> Include<TElement>(Expression<Func<T, ICollection<TElement>>> navigation);

    ISpecificationBuilder<T> Skip(int skip);
    ISpecificationBuilder<T> Take(int take);

    ISpecificationBuilder<T> AsNoTracking();
    ISpecificationBuilder<T> AsTracking();
}



public interface ISpecificationBuilder<T, TResult>
{
    ISpecificationBuilder<T, TResult> Where(Expression<Func<T, bool>> predicate);

    IOrderedSpecificationBuilder<T, TResult> OrderBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T, TResult> OrderByDescending(Expression<Func<T, object?>> orderExpression);

    ISpecificationBuilder<T, TResult> Skip(int skip);
    ISpecificationBuilder<T, TResult> Take(int take);

    ISpecificationBuilder<T, TResult> AsNoTracking();
    ISpecificationBuilder<T, TResult> AsTracking();

    ISpecificationBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector);
    ISpecificationBuilder<T, TResult> SelectMany(Expression<Func<T, IEnumerable<TResult>>> selector);
}
