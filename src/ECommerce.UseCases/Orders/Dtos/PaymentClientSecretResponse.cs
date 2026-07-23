namespace ECommerce.UseCases.Orders.Dtos;

public sealed record PaymentClientSecretResponse(
    Guid OrderId,
    string PaymentIntentId,
    string ClientSecret,
    string PublishableKey);
