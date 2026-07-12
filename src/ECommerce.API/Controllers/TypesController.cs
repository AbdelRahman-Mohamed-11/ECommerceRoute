using ECommerce.API.Models;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Types.Dtos;
using ECommerce.UseCases.Types.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

public class TypesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetAllTypesResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllTypesResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAllTypesQuery(), ct);

        if (result.IsFailure)
            return Problem(result);

        return Ok(ApiResponse<IReadOnlyList<GetAllTypesResponse>>.Ok(result.Value, HttpContext.TraceIdentifier));
    }
}
