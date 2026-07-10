using ECommerce.Domain.Specifications;
using ECommerce.UseCases.Specifications.Includes;
using ECommerce.UseCases.Specifications.Orders;
using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications;

// SpecificationBuilder<T> — Application implementation of the fluent API.
// Responsibility: each method writes into Specification<T> and returns this (or a chain builder).
public class SpecificationBuilder<T> : ISpecificationBuilder<T>
{
    protected readonly Specification<T> Specification;

    internal SpecificationBuilder(Specification<T> specification)
        => Specification = specification;


    public ISpecificationBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        Specification.AddWhere(predicate);
        return this;
    }

    public IIncludableSpecificationBuilder<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> navigation)
    {
        var parent = Specification.AddInclude(navigation);

        return new IncludableSpecificationBuilder<T, TProperty>(Specification, parent);
    }

    public IIncludableCollectionSpecificationBuilder<T, TElement> Include<TElement>(Expression<Func<T, ICollection<TElement>>> navigation)
    {
        var parent = Specification.AddInclude(navigation);
        return new IncludableCollectionSpecificationBuilder<T, TElement>(Specification, parent);
    }


    public IOrderedSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new Domain.Specifications.OrderExpressionInfo<T>(orderExpression, Domain.Specifications.OrderType.OrderBy));
        return new OrderedSpecificationBuilder<T>(Specification);
    }


    public IOrderedSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new Domain.Specifications.OrderExpressionInfo<T>(orderExpression, Domain.Specifications.OrderType.OrderByDescending));
        return new OrderedSpecificationBuilder<T>(Specification);
    }


    public ISpecificationBuilder<T> Skip(int skip)
    {
        Specification.SetSkip(skip);
        return this;
    }

    public ISpecificationBuilder<T> Take(int take)
    {
        Specification.SetTake(take);
        return this;
    }

    public ISpecificationBuilder<T> AsNoTracking()
    {
        Specification.SetNoTracking();
        return this;
    }

    public ISpecificationBuilder<T> AsTracking()
    {
        Specification.SetTracking();
        return this;
    }
}

public class SpecificationBuilder<T, TResult> : ISpecificationBuilder<T, TResult>
{
    protected readonly Specification<T, TResult> Specification;
    private readonly SpecificationBuilder<T> _builder;

    internal SpecificationBuilder(Specification<T, TResult> specification)
    {
        Specification = specification;
        _builder = new SpecificationBuilder<T>(specification);
    }

    public ISpecificationBuilder<T, TResult> Where(Expression<Func<T, bool>> predicate)
    {
        _builder.Where(predicate);
        return this;
    }

    public IOrderedSpecificationBuilder<T, TResult> OrderBy(Expression<Func<T, object?>> orderExpression)
    {
        _builder.OrderBy(orderExpression);
        return new OrderedSpecificationBuilder<T, TResult>(Specification);
    }

    public IOrderedSpecificationBuilder<T, TResult> OrderByDescending(Expression<Func<T, object?>> orderExpression)
    {
        _builder.OrderByDescending(orderExpression);
        return new OrderedSpecificationBuilder<T, TResult>(Specification);
    }

    public ISpecificationBuilder<T, TResult> Skip(int skip)
    {
        _builder.Skip(skip); return this;
    }

    public ISpecificationBuilder<T, TResult> Take(int take)
    {
        _builder.Take(take); return this;
    }

    public ISpecificationBuilder<T, TResult> AsNoTracking()
    {
        _builder.AsNoTracking(); return this;
    }

    public ISpecificationBuilder<T, TResult> AsTracking()
    {
        _builder.AsTracking(); return this;
    }

    public ISpecificationBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector)
    {
        Specification.SetSelector(selector);
        return this;
    }

    public ISpecificationBuilder<T, TResult> SelectMany(Expression<Func<T, IEnumerable<TResult>>> selector)
    {
        Specification.SetSelectMany(selector);
        return this;
    }
}



