using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.DeliveryMethods.Commands.CreateDeliveryMethod;

public sealed class CreateDeliveryMethodCommandHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDeliveryMethodCommand, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> Handle(
        CreateDeliveryMethodCommand request,
        CancellationToken cancellationToken)
    {
        if (await repository.AnyAsync(new DeliveryMethodByNameSpecification(request.Name), cancellationToken))
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NameAlreadyExists);

        var createResult = DeliveryMethod.Create(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.EstimatedDeliveryTime,
            request.Description,
            request.IsAvailable,
            request.DisplayOrder);

        if (createResult.IsFailure)
            return Result<DeliveryMethodResponse>.Failure(createResult.Error);

        var entity = createResult.Value;
        repository.Add(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DeliveryMethodResponse>.Success(ToResponse(entity));
    }

    private static DeliveryMethodResponse ToResponse(DeliveryMethod m) =>
        new(m.Id, m.Name, m.Description, m.Price, m.EstimatedDeliveryTime, m.IsAvailable, m.DisplayOrder);
}
