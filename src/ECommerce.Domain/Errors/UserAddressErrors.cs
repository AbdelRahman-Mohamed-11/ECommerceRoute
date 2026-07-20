using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class UserAddressErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "UserAddress.NotFound",
            "Address was not found.");

    public static readonly Error InvalidId =
        Error.Validation(
            "UserAddress.InvalidId",
            "Address id is required.");

    public static readonly Error InvalidUserId =
        Error.Validation(
            "UserAddress.InvalidUserId",
            "User id is required.");


    public static readonly Error InvalidName =
        Error.Validation(
            "UserAddress.InvalidName",
            "Recipient name is required.");

    public static readonly Error InvalidPhone =
        Error.Validation(
            "UserAddress.InvalidPhone",
            "Phone number is required.");

    public static readonly Error InvalidLocation =
        Error.Validation(
            "UserAddress.InvalidLocation",
            "Country, city and street are required.");

    public static readonly Error InvalidPostalCode =
        Error.Validation(
            "UserAddress.InvalidPostalCode",
            "Postal code is required.");
}

