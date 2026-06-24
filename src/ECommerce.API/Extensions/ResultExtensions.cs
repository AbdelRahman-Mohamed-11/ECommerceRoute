using ECommerce.Domain.Shared;

namespace ECommerce.API.Extensions;

public static class ResultExtensions
{
    public static int ToStatusCode(this Error error) =>
        error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

    public static int ToStatusCode(this SuccessType successType) =>
        successType switch
        {
            SuccessType.Ok => StatusCodes.Status200OK,
            SuccessType.Created => StatusCodes.Status201Created,
            SuccessType.NoContent => StatusCodes.Status204NoContent,
            _ => StatusCodes.Status200OK
        };
}
