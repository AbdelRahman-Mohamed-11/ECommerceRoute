using ECommerce.API.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;

namespace ECommerce.API.Extensions;

public static class ResultExtensions
{
    public static IResult Success<T>(
        T data,
        HttpContext context,
        string successMessage,
        PaginationMeta? pagination = null)
    {
        var response = ApiResponse<T>.Ok(data, context.TraceIdentifier, successMessage, pagination);

        return Results.Ok(response);
    }

    public static IResult FromResult(
        this Result result,
        HttpContext context,
        string successMessage)
    {
        return result.IsFailure
            ? Problem(result, context)
            : Success<object?>(null, context, successMessage);
    }

    public static IResult FromResult<T>(
        this Result<T> result,
        HttpContext context,
        string successMessage,
        PaginationMeta? pagination = null)
    {
        return result.IsFailure
            ? Problem(result, context)
            : Success(result.Value, context, successMessage, pagination);
    }

    public static IResult FromPagedResult<T>(
        this Result<PagedResult<T>> result,
        HttpContext context,
        int pageNumber,
        int pageSize,
        string successMessage)
    {
        return result.IsFailure
            ? Problem(result, context)
            : Success(result.Value.Items, context, successMessage, new PaginationMeta(pageNumber, pageSize, result.Value.TotalCount));
    }

    public static IResult Problem(
        this Result result,
        HttpContext context)
    {
        var statusCode = result.Error.ToStatusCode();
        var title = result.Error.ToTitle();

        var problem = new Dictionary<string, object?>
        {
            ["type"] = $"https://example.com/errors/{result.Error.Code}",
            ["title"] = title,
            ["status"] = statusCode,
        };

        if (result.Error.Type is ErrorType.Validation)
        {
            problem["errors"] = new Dictionary<string, string[]>
            {
                [result.Error.Code] = [result.Error.Message]
            };
        }
        else
        {
            problem["detail"] = result.Error.Message;
        }

        problem["traceId"] = context.TraceIdentifier;

        return Results.Json(problem, statusCode: statusCode);
    }

    public static IResult Problem<T>(
        this Result<T> result,
        HttpContext context) =>
        Problem((Result)result, context);

    private static int ToStatusCode(this Error error) =>
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

    private static string ToTitle(this Error error) =>
        error.Type switch
        {
            ErrorType.Validation => "Validation Error",
            ErrorType.NotFound => "Resource Not Found",
            ErrorType.Unauthorized => "Unauthorized Access",
            ErrorType.Forbidden => "Forbidden Access",
            ErrorType.Conflict => "Conflict Error",
            _ => "Internal Server Error"
        };
}
