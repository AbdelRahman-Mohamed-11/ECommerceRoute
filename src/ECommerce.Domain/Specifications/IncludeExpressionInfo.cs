using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

public sealed record IncludeExpressionInfo(
    LambdaExpression LambdaExpression,
    LambdaExpression PreviousExpression);
