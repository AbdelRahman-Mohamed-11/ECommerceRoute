using System.Text.Json.Serialization;

namespace ECommerce.API.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;

    public string? Message { get; set; }

    public T? Data { get; set; }

    public ApiMeta Meta { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string traceId, string? message = null, PaginationMeta? pagination = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Meta = new ApiMeta
            {
                TraceId = traceId,
                Pagination = pagination
            },
            Message = message
        };
    }
}

public class ApiMeta
{
    public string TraceId { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationMeta? Pagination { get; set; }
}

public class PaginationMeta
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }

    public PaginationMeta(int pageNumber, int pageSize, int totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }
}