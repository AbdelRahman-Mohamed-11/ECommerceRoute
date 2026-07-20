namespace ECommerce.UseCases.Common.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResult GenerateToken(Guid userId, string email, string? displayName,
        IEnumerable<string> roles);
}
