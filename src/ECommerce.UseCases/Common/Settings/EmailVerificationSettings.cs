namespace ECommerce.UseCases.Common.Settings;

public sealed class EmailVerificationSettings
{
    public const string SectionName = "EmailVerification";

    public int CodeLength { get; set; } = 6;
    public int ExpirationMinutes { get; set; } = 15;
    public int MaxFailedAttempts { get; set; } = 5;
}
