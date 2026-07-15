using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Enums;

namespace ECommerce.UseCases.Products.Queries;

// usecase: 1. query/command 2. handler  3. spec
public sealed record GetPagedProductsQuery(
        int PageNumber = 1,
        int PageSize = 5,
        string? Search = null,
        Guid? BrandId = null,
        Guid? TypeId = null,
        ProductSortField? SortBy = ProductSortField.Name,
        bool SortDescending = false
    ) : IQuery<Result<PagedResult<GetAllProductsResponse>>>;
