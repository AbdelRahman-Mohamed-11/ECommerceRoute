using ECommerce.UseCases.Specifications;
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


public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T>
{
    IOrderedSpecificationBuilder<T> ThenBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T>  ThenByDescending(Expression<Func<T, object?>> orderExpression);
}


public interface IIncludableSpecificationBuilder<T, TProperty> : ISpecificationBuilder<T>
{
    // o => o.Customer => Expression<Func<Order, Customer>>  T: Order, TProperty: Customer, TNext:Address

    IIncludableSpecificationBuilder<T,TNext> ThenInclude<TNext>(
            Expression<Func<TProperty, TNext>> navigation);  // Expression<Order,Address>
}
// include(o => o.Customer)  T: Order   TProperty:Customer
// theninclude(c => c.Address
// )


// Include(o => o.Items)  // ICollection<OrderItem>   TElement: OrderItem, TNext: Product
// .thenicnlude(i => i.Product)

public interface IIncludableCollectionSpecificationBuilder<T, TElement> 
    : ISpecificationBuilder<T>
{
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(Expression<Func<TElement, TNext>> navigation);
}



public interface ISpecificationBuilder<T, TResult>
{
    ISpecificationBuilder<T, TResult> Where(Expression<Func<T, bool>> predicate);

    IOrderedSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderExpression);


    ISpecificationBuilder<T, TResult> Skip(int skip);
    ISpecificationBuilder<T, TResult> Take(int take);

    ISpecificationBuilder<T, TResult> AsNoTracking();
    ISpecificationBuilder<T, TResult> AsTracking();


    ISpecificationBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector);
    ISpecificationBuilder<T, TResult> SelectMany(Expression<Func<T, IEnumerable<TResult>>> selector);
}