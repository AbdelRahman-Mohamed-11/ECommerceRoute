using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Identity.Commands.AddUserAddress;

public sealed class AddUserAddressCommandHandler(
    ICurrentUserService currentUser,
    IRepository<UserAddress> addressRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddUserAddressCommand, Result<UserAddressResponse>>
{
    public async Task<Result<UserAddressResponse>> Handle(
        AddUserAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<UserAddressResponse>.Failure(IdentityErrors.InvalidCredentials);

        var createResult = UserAddress.Create(
            Guid.NewGuid(),
            currentUser.UserId.Value,
            request.Label,
            request.RecipientFirstName,
            request.RecipientLastName,
            request.PhoneNumber,
            request.Country,
            request.City,
            request.Street,
            request.PostalCode,
            request.IsDefaultShipping,
            request.IsDefaultBilling);

        if (createResult.IsFailure)
            return Result<UserAddressResponse>.Failure(createResult.Error);

        var address = createResult.Value;
        addressRepository.Add(address);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserAddressResponse>.Success(new UserAddressResponse(
            address.Id,
            address.Label,
            address.RecipientFirstName,
            address.RecipientLastName,
            address.PhoneNumber,
            address.Country,
            address.City,
            address.Street,
            address.PostalCode,
            address.IsDefaultShipping,
            address.IsDefaultBilling));
    }
}
