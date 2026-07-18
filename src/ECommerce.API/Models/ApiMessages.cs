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
    public const string VerificationCodeSent =
        "Registration successful. A verification code has been sent to your email. Confirm your email before logging in.";
    public const string VerificationCodeResent =
        "This email is registered but not confirmed. A new verification code has been sent to your email.";
    public const string EmailConfirmed = "Email confirmed successfully.";
}
