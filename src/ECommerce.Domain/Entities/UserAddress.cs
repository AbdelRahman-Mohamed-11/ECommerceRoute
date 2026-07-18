using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public sealed class UserAddress : BaseEntity
{
    public const int MaxLabelLength = 64;
    public const int MaxNameLength = 100;
    public const int MaxPhoneLength = 32;
    public const int MaxCountryLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 200;
    public const int MaxPostalCodeLength = 20;

    private UserAddress()
    {
    }

    public Guid UserId { get; private set; }
    public string Label { get; private set; } = null!;
    public string RecipientFirstName { get; private set; } = null!;
    public string RecipientLastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Street { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public bool IsDefaultShipping { get; private set; }
    public bool IsDefaultBilling { get; private set; }

    public static Result<UserAddress> Create(
        Guid id,
        Guid userId,
        string label,
        string recipientFirstName,
        string recipientLastName,
        string phoneNumber,
        string country,
        string city,
        string street,
        string postalCode,
        bool isDefaultShipping = false,
        bool isDefaultBilling = false)
    {
        if (id == Guid.Empty)
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidId", "Address id is required."));

        if (userId == Guid.Empty)
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidUserId", "User id is required."));

        if (string.IsNullOrWhiteSpace(label))
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidLabel", "Label is required."));

        if (string.IsNullOrWhiteSpace(recipientFirstName) || string.IsNullOrWhiteSpace(recipientLastName))
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidName", "Recipient name is required."));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidPhone", "Phone number is required."));

        if (string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street))
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidLocation", "Country, city and street are required."));

        if (string.IsNullOrWhiteSpace(postalCode))
            return Result<UserAddress>.Failure(Error.Validation("Address.InvalidPostalCode", "Postal code is required."));

        return Result<UserAddress>.Success(new UserAddress
        {
            Id = id,
            UserId = userId,
            Label = label.Trim(),
            RecipientFirstName = recipientFirstName.Trim(),
            RecipientLastName = recipientLastName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Country = country.Trim(),
            City = city.Trim(),
            Street = street.Trim(),
            PostalCode = postalCode.Trim(),
            IsDefaultShipping = isDefaultShipping,
            IsDefaultBilling = isDefaultBilling,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }
}
