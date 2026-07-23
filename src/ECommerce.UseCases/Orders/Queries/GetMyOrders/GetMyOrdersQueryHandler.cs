using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler(
    ICurrentUserService currentUser,
    IReadRepository<Order> orderRepository)
    : IRequestHandler<GetMyOrdersQuery, Result<PagedResult<OrderResponse>>>
{
    public async Task<Result<PagedResult<OrderResponse>>> Handle(
        GetMyOrdersQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<PagedResult<OrderResponse>>.Failure(OrderErrors.Unauthorized);

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > 50 ? 10 : request.PageSize;
        var userId = currentUser.UserId.Value;

        var totalCount = await orderRepository.CountAsync(
            new OrdersByUserIdCountSpecification(userId),
            cancellationToken);

        var orders = await orderRepository.ListAsync(
            new OrdersByUserIdPagedSpecification(userId, pageNumber, pageSize),
            cancellationToken);

        var items = orders.Select(OrderMappings.ToResponse).ToList();

        return Result<PagedResult<OrderResponse>>.Success(
            new PagedResult<OrderResponse>(items, totalCount));
    }
}
