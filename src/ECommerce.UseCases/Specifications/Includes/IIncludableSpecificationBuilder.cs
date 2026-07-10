using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Includes;

public interface IIncludableSpecificationBuilder<T, TProperty> : ISpecificationBuilder<T>
{
    // T : Order, TNext: Address
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(
            Expression<Func<TProperty, TNext>> navigation);  // C => C.Address
}

public interface IIncludableCollectionSpecificationBuilder<T, TElement>
    : ISpecificationBuilder<T>
{
    // Include (o => o.Items)
    // .ThenInclude(item => item.Product)    TElement: ITEM , TNext : Product
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(Expression<Func<TElement, TNext>> navigation);
}