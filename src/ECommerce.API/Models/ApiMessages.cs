namespace ECommerce.API.Models;

public static class ApiMessages
{
    public const string BasketRetrieved = "Basket retrieved successfully.";
    public const string BasketItemAdded = "Item added to basket successfully.";
    public const string BasketItemUpdated = "Basket item quantity updated successfully.";
    public const string BasketItemRemoved = "Item removed from basket successfully.";
    public const string BasketCleared = "Basket cleared successfully.";
    public const string BasketMerged = "Anonymous basket merged successfully.";
    public const string Registered = "Registered successfully.";
    public const string LoggedIn = "Logged in successfully.";
    public const string TokenRefreshed = "Tokens refreshed successfully.";
    public const string LoggedOut = "Logged out successfully.";
    public const string VerificationCodeSent =
        "Registration successful. A verification code was sent to your email. Confirm your email before logging in.";
    public const string VerificationCodeResent =
        "This email is registered but not confirmed. A new verification code was sent to your email.";
    public const string EmailConfirmed = "Email confirmed successfully.";
    public const string CurrentUserRetrieved = "Current user retrieved successfully.";
    public const string UserProfileUpdated = "Profile updated successfully.";
    public const string UserAddressesRetrieved = "Addresses retrieved successfully.";
    public const string UserAddressAdded = "Address added successfully.";

    public const string OrderCreated = "Order created successfully.";
    public const string OrdersRetrieved = "Orders retrieved successfully.";
    public const string OrderRetrieved = "Order retrieved successfully.";
    public const string OrderCancelled = "Order cancelled successfully.";

    public const string DeliveryMethodsRetrieved = "Delivery methods retrieved successfully.";
    public const string DeliveryMethodRetrieved = "Delivery method retrieved successfully.";
    public const string DeliveryMethodCreated = "Delivery method created successfully.";
    public const string DeliveryMethodUpdated = "Delivery method updated successfully.";
    public const string DeliveryMethodDeleted = "Delivery method deleted successfully.";
}
