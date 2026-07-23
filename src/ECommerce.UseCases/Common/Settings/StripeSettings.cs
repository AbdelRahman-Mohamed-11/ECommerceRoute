namespace ECommerce.UseCases.Common.Settings;

public sealed class StripeSettings
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = null!;
    public string PublishableKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;

    /// <summary>e.g. usd — must match Stripe account capabilities.</summary>
    public string Currency { get; set; } = "usd";
}
