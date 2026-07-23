using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;

public sealed class UpdateDeliveryMethodCommandHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDeliveryMethodCommand, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> Handle(
        UpdateDeliveryMethodCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NotFound);

        if (await repository.AnyAsync(
                new DeliveryMethodByNameSpecification(request.Name, request.Id),
                cancellationToken))
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NameAlreadyExists);

        var updateResult = entity.Update(
            request.Name,
            request.Price,
            request.EstimatedDeliveryTime,
            request.Description,
            request.IsAvailable,
            request.DisplayOrder);

        if (updateResult.IsFailure)
            return Result<DeliveryMethodResponse>.Failure(updateResult.Error);

        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DeliveryMethodResponse>.Success(
            new DeliveryMethodResponse(
                entity.Id,
                entity.Name,
                entity.Description,
                entity.Price,
                entity.EstimatedDeliveryTime,
                entity.IsAvailable,
                entity.DisplayOrder));
    }
}
