using ECommerce.API.Models;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

// CQRS, Mediator
public class ProductsController(
        GetAllProductsQuery getAllProductsQuery,
        GetByIdProductQuery getByIdProductQuery) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetAllProductsResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await getAllProductsQuery.ExecuteAsync(ct);

        if (result.IsFailure)
            return Problem(result);

        return Ok(ApiResponse<IReadOnlyList<GetAllProductsResponse>>.Ok(result.Value, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="ct">A cancellation token used to cancel the request.</param>
    /// <returns>
    /// Returns the product details if found.
    /// </returns>
    /// <response code="200">Product was found successfully.</response>
    /// <response code="404">Product was not found.</response>
    [HttpGet("{id:guid}")] // Get API/Products/{id}
    [ProducesResponseType(typeof(ApiResponse<GetByIdProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetByIdProductResponse>>> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await getByIdProductQuery.ExecuteAsync(id, ct);

        if (result.IsFailure)
            return Problem(result);

        return Ok(ApiResponse<GetByIdProductResponse>.Ok(result.Value, HttpContext.TraceIdentifier));
    }
}