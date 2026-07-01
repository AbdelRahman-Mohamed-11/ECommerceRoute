using ECommerce.API.Models;
using ECommerce.UseCases.Types.Dtos;
using ECommerce.UseCases.Types.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

public class TypesController(GetAllTypesQuery getAllTypesQuery) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetAllTypesResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllTypesResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await getAllTypesQuery.ExecuteAsync(ct);

        return FromResult(result);
    }
}