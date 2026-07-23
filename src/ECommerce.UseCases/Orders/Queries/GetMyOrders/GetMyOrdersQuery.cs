using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Dtos;

namespace ECommerce.UseCases.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10) : IQuery<Result<PagedResult<OrderResponse>>>;
