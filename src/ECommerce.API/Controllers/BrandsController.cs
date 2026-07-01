using ECommerce.API.Models;
using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Brands.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

public class BrandsController(GetAllBrandsQuery getAllBrandsQuery) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetAllBrandsResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllBrandsResponse>>>> GetAll(CancellationToken ct = default)
    {
        var result = await getAllBrandsQuery.ExecuteAsync(ct);

        return FromResult(result);
    }
}