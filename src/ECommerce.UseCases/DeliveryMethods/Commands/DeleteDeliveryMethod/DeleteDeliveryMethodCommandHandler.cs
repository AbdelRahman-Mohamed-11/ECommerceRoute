using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.DeliveryMethods.Commands.DeleteDeliveryMethod;

public sealed class DeleteDeliveryMethodCommandHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteDeliveryMethodCommand, Result>
{
    public async Task<Result> Handle(
        DeleteDeliveryMethodCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure(DeliveryMethodErrors.NotFound);

        repository.Delete(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
