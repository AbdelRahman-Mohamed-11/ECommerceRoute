namespace ECommerce.API.Test;

public sealed record GenerateTestJwtRequest(
Guid UserId,
string Email,
string displayName,
string[] Roles);