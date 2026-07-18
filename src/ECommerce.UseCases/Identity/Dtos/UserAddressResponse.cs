namespace ECommerce.UseCases.Identity.Dtos;

public sealed record UserAddressResponse(
    Guid Id,
    string Label,
    string RecipientFirstName,
    string RecipientLastName,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string PostalCode,
    bool IsDefaultShipping,
    bool IsDefaultBilling);
