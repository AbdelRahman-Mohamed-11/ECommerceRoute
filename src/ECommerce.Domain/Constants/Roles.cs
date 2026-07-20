namespace ECommerce.Domain.Constants;

// SuperAdmin - Admin - User      - Manager
public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly IReadOnlyList<string> All =
    [
        SuperAdmin,
        Admin,
        User
    ];
}