using ECommerce.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Specifications;

// SpecificationEvaluator — Infrastructure (EF Core glue).
// Responsibility: turns ISpecification<T> into IQueryable<T>. Only place that references EF.
// Applies: tracking → Where → Include → OrderBy → (optional Skip/Take) → Select (for projection specs).
public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> source,
        ISpecification<T> specification,
        bool ignorePaging = false)
        where T : class
    {
        var query = source;

        // 1. Tracking mode
        query = specification.IsTrackingEnabled ? query.AsTracking() : query.AsNoTracking();

        // 2. Filters — each WhereExpression becomes query.Where(...)
        foreach (var whereExpression in specification.WhereExpressions)
            query = query.Where(whereExpression);

        // 3. Includes — each ThenInclude extends the specific chain by expression reference
        query = ApplyIncludes(query, specification);

        // 4. Order — always applied (independent of pagination; OrderBy without Skip/Take is valid)
        query = ApplyOrdering(query, specification);

        // 5. Page — only Skip/Take; skipped when counting (CountAsync / AnyAsync use ignorePaging: true)
        if (!ignorePaging)
            query = ApplyPaging(query, specification);

        return query;
    }

    // Projection overload — TResult is the DTO from Selector / SelectorMany
    public static IQueryable<TResult> GetQuery<T, TResult>(
        IQueryable<T> source,
        ISpecification<T, TResult> specification)
        where T : class
    {
        var query = GetQuery(source, (ISpecification<T>)specification);

        return specification.Selector is not null
            ? query.Select(specification.Selector)
            : query.SelectMany(specification.SelectorMany!);
    }

    // Track each chain's current IIncludableQueryable by the exact Include/ThenInclude expression
    // that created it — reference identity, so parallel chains never collide by type.
    private static IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, ISpecification<T> specification)
        where T : class
    {
        var chainHeads = new Dictionary<LambdaExpression, object>();

        foreach (var include in specification.Includes)
        {
            var head = query.Include(include);
            chainHeads[include] = head;
            query = head;
        }

        foreach (var thenInclude in specification.IncludeExpressions)
        {
            if (!chainHeads.TryGetValue(thenInclude.PreviousExpression, out var head))
                throw new InvalidOperationException("No matching Include chain found.");

            var next = ((dynamic)head).ThenInclude((dynamic)thenInclude.LambdaExpression);
            chainHeads[thenInclude.LambdaExpression] = next;
            query = next;
        }

        return query;
    }

    private static IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.OrderExpressions.Count == 0)
            return query;

        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var order in specification.OrderExpressions)
        {
            orderedQuery = order.OrderType switch
            {
                OrderType.OrderBy => query.OrderBy(order.KeySelector),
                OrderType.OrderByDescending => query.OrderByDescending(order.KeySelector),
                OrderType.ThenBy => orderedQuery!.ThenBy(order.KeySelector),
                OrderType.ThenByDescending => orderedQuery!.ThenByDescending(order.KeySelector),
                _ => throw new ArgumentOutOfRangeException(nameof(order.OrderType))
            };

            query = orderedQuery;
        }

        return orderedQuery!;
    }

    private static IQueryable<T> ApplyPaging<T>(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        return query;
    }
}
