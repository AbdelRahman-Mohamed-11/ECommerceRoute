using Asp.Versioning;
using ECommerce.API.Models;
using ECommerce.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(
        T data,
        string message,
        PaginationMeta? pagination = null) =>
        Ok(ApiResponse<T>.Ok(data, HttpContext.TraceIdentifier, message, pagination));


    protected ActionResult<ApiResponse<T>> FromResult<T>(
       Result<T> result,
       string successMessage,
       PaginationMeta? pagination = null) =>
       result.IsFailure
           ? Problem(result)
           : Success(result.Value, successMessage, pagination);

    protected ActionResult Problem(Result result)
    {
        var statusCode = result.Error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var title = result.Error.Type switch
        {
            ErrorType.Validation => "Validation Error",
            ErrorType.NotFound => "Resource Not Found",
            ErrorType.Unauthorized => "Unauthorized Access",
            ErrorType.Forbidden => "Forbidden Access",
            ErrorType.Conflict => "Conflict Error",
            _ => "Internal Server Error"
        };

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

        problem["traceId"] = HttpContext.TraceIdentifier;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }
}
