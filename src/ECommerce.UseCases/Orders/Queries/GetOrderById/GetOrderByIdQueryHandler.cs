using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    ICurrentUserService currentUser,
    IReadRepository<Order> orderRepository)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<OrderResponse>.Failure(OrderErrors.Unauthorized);

        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByIdForUserSpecification(request.OrderId, currentUser.UserId.Value),
            cancellationToken);

        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFound);

        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}
