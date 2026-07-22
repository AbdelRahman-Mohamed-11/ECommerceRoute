namespace ECommerce.UseCases.Common.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "Email";

    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 25;
    public string? Username { get; set; }
    public string? Password { get; set; }
}
