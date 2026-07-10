using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

// Expression<Func<T,TResult>>
public sealed record IncludeExpressionInfo(
        LambdaExpression LambdaExpression, // ThenInclude(p => p.sfs)
        LambdaExpression PreviousExpression // Include(p => p...)
    );
