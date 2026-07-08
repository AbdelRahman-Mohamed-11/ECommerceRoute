using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Includes;

public interface IIncludableSpecificationBuilder<T, TProperty> : ISpecificationBuilder<T>
{
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(
            Expression<Func<TProperty, TNext>> navigation);
}

public interface IIncludableCollectionSpecificationBuilder<T, TElement>
    : ISpecificationBuilder<T>
{
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(Expression<Func<TElement, TNext>> navigation);
}