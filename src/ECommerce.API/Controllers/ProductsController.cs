using ECommerce.API.Models;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace ECommerce.API.Controllers;

public class ProductsController(ISender sender) : ApiControllerBase
{
    [OutputCache(Duration = 20, Tags = ["Products"])]
    [HttpGet("paged")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetAllProductsResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>> Paged(
       [FromQuery] GetPagedProductsQuery query,
        CancellationToken ct = default)
    {
        var result = await sender.Send(query, ct);

        if (result.IsFailure)
            return Problem(result);

        return FromPagedResult(result, query.PageNumber, query.PageSize, "Paged products retrieved succefully");
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
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetByIdProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetByIdProductResponse>>> GetById(Guid id, CancellationToken ct = default)
    {
        // MEDIR
        var result = await sender.Send(new GetByIdProductQuery(id), ct);

        if (result.IsFailure)
            return Problem(result);

        return Ok(ApiResponse<GetByIdProductResponse>.Ok(result.Value, HttpContext.TraceIdentifier));
    }



    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromForm] CreateProductCommand command,
        CancellationToken ct = default)
    {
        var result = await sender.Send(command, ct);

        if (result.IsFailure)
            return Problem(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            ApiResponse<Guid>.Ok(result.Value, HttpContext.TraceIdentifier));
    }
}
